using SecurityContext = ASC.Core.SecurityContext;
using CrmDaoFactory = ASC.CRM.Core.Dao.DaoFactory;
using ASC.CRM.Core;

namespace ASC.Mail.Core.Engine;

[Scope]
public class CrmLinkEngine
{
    private int Tenant => _tenantManager.GetCurrentTenant().TenantId;
    private string User => _securityContext.CurrentAccount.ID.ToString();

    private readonly ILogger _log;
    private readonly SecurityContext _securityContext;
    private readonly TenantManager _tenantManager;
    private readonly ApiHelper _apiHelper;
    private readonly IMailDaoFactory _mailDaoFactory;
    private readonly MessageEngine _messageEngine;
    private readonly StorageFactory _storageFactory;
    private readonly CrmSecurity _crmSecurity;
    private readonly IServiceProvider _serviceProvider;

    public CrmLinkEngine(
        SecurityContext securityContext,
        TenantManager tenantManager,
        ApiHelper apiHelper,
        IMailDaoFactory mailDaoFactory,
        MessageEngine messageEngine,
        StorageFactory storageFactory,
        ILoggerProvider logProvider,
        CrmSecurity crmSecurity,
        IServiceProvider serviceProvider)
    {
        _securityContext = securityContext;
        _tenantManager = tenantManager;
        _apiHelper = apiHelper;
        _mailDaoFactory = mailDaoFactory;
        _messageEngine = messageEngine;
        _storageFactory = storageFactory;
        _serviceProvider = serviceProvider;
        _crmSecurity = crmSecurity;
        _log = logProvider.CreateLogger("ASC.Mail.CrmLinkEngine");
    }

    public List<CrmContactData> GetLinkedCrmEntitiesId(int messageId)
    {
        var mail = _mailDaoFactory.GetMailDao().GetMail(new ConcreteUserMessageExp(messageId, Tenant, User));

        return _mailDaoFactory.GetCrmLinkDao().GetLinkedCrmContactEntities(mail.ChainId, mail.MailboxId);
    }

    public void LinkChainToCrm(int messageId, List<CrmContactData> contactIds)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var factory = scope.ServiceProvider.GetService<CrmDaoFactory>();
            foreach (var crmContactEntity in contactIds)
            {
                switch (crmContactEntity.Type)
                {
                    case CrmContactData.EntityTypes.Contact:
                        var crmContact = factory.GetContactDao().GetByID(crmContactEntity.Id);
                        _crmSecurity.DemandAccessTo(crmContact);
                        break;
                    case CrmContactData.EntityTypes.Case:
                        var crmCase = factory.GetCasesDao().GetByID(crmContactEntity.Id);
                        _crmSecurity.DemandAccessTo(crmCase);
                        break;
                    case CrmContactData.EntityTypes.Opportunity:
                        var crmOpportunity = factory.GetDealDao().GetByID(crmContactEntity.Id);
                        _crmSecurity.DemandAccessTo(crmOpportunity);
                        break;
                }
            }
        }

        var mail = _mailDaoFactory.GetMailDao().GetMail(new ConcreteUserMessageExp(messageId, Tenant, User));

        var chainedMessages = _mailDaoFactory.GetMailInfoDao().GetMailInfoList(
            SimpleMessagesExp.CreateBuilder(Tenant, User)
                .SetChainId(mail.ChainId)
                .Build());

        if (!chainedMessages.Any())
            return;

        var linkingMessages = new List<MailMessageData>();

        foreach (var chainedMessage in chainedMessages)
        {
            var message = _messageEngine.GetMessage(chainedMessage.Id,
                new MailMessageData.Options
                {
                    LoadImages = true,
                    LoadBody = true,
                    NeedProxyHttp = false
                });

            message.LinkedCrmEntityIds = contactIds;

            linkingMessages.Add(message);

        }

        var strategy = _mailDaoFactory.GetContext().Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = _mailDaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted);

            _mailDaoFactory.GetCrmLinkDao().SaveCrmLinks(mail.ChainId, mail.MailboxId, contactIds);

            foreach (var message in linkingMessages)
            {
                try
                {
                    AddRelationshipEvents(message);
                }
                catch (ApiHelperException ex)
                {
                    if (!ex.Message.Equals("Already exists"))
                        throw;
                }
            }

            tx.Commit();
        });
    }

    public void MarkChainAsCrmLinked(int messageId, List<CrmContactData> contactIds)
    {
        var strategy = _mailDaoFactory.GetContext().Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = _mailDaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted);

            var mail = _mailDaoFactory.GetMailDao().GetMail(new ConcreteUserMessageExp(messageId, Tenant, User));

            _mailDaoFactory.GetCrmLinkDao().SaveCrmLinks(mail.ChainId, mail.MailboxId, contactIds);

            tx.Commit();
        });
    }

    public void UnmarkChainAsCrmLinked(int messageId, IEnumerable<CrmContactData> contactIds)
    {
        var strategy = _mailDaoFactory.GetContext().Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = _mailDaoFactory.BeginTransaction(IsolationLevel.ReadUncommitted);

            var mail = _mailDaoFactory.GetMailDao().GetMail(new ConcreteUserMessageExp(messageId, Tenant, User));

            _mailDaoFactory.GetCrmLinkDao().RemoveCrmLinks(mail.ChainId, mail.MailboxId, contactIds);

            tx.Commit();
        });
    }

    public void ExportMessageToCrm(int messageId, IEnumerable<CrmContactData> crmContactIds)
    {
        if (messageId < 0)
            throw new ArgumentException(@"Invalid message id", nameof(messageId));
        if (crmContactIds == null)
            throw new ArgumentException(@"Invalid contact ids list", nameof(crmContactIds));

        var messageItem = _messageEngine.GetMessage(messageId, new MailMessageData.Options
        {
            LoadImages = true,
            LoadBody = true,
            NeedProxyHttp = false
        });

        messageItem.LinkedCrmEntityIds = crmContactIds.ToList();

        AddRelationshipEvents(messageItem);
    }

    public void AddRelationshipEventForLinkedAccounts(MailBoxData mailbox, MailMessageData messageItem)
    {
        try
        {
            messageItem.LinkedCrmEntityIds = _mailDaoFactory.GetCrmLinkDao()
                .GetLinkedCrmContactEntities(messageItem.ChainId, mailbox.MailBoxId);

            if (!messageItem.LinkedCrmEntityIds.Any()) return;

            AddRelationshipEvents(messageItem, mailbox);
        }
        catch (Exception ex)
        {
            _log.WarnCrmLinkEngineAddingHistoryEvent(messageItem.Id, ex);
        }
    }

    public void AddRelationshipEvents(MailMessageData message, MailBoxData mailbox = null)
    {
        using var scope = _serviceProvider.CreateScope();

        if (mailbox != null)
        {
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var securityContext = scope.ServiceProvider.GetService<SecurityContext>();

            tenantManager.SetCurrentTenant(mailbox.TenantId);
            securityContext.AuthenticateMe(new Guid(mailbox.UserId));
        }

        var factory = scope.ServiceProvider.GetService<CrmDaoFactory>();
        foreach (var contactEntity in message.LinkedCrmEntityIds)
        {
            switch (contactEntity.Type)
            {
                case CrmContactData.EntityTypes.Contact:
                    var crmContact = factory.GetContactDao().GetByID(contactEntity.Id);
                    _crmSecurity.DemandAccessTo(crmContact);
                    break;
                case CrmContactData.EntityTypes.Case:
                    var crmCase = factory.GetCasesDao().GetByID(contactEntity.Id);
                    _crmSecurity.DemandAccessTo(crmCase);
                    break;
                case CrmContactData.EntityTypes.Opportunity:
                    var crmOpportunity = factory.GetDealDao().GetByID(contactEntity.Id);
                    _crmSecurity.DemandAccessTo(crmOpportunity);
                    break;
            }

            var fileIds = new List<object>();

            foreach (var attachment in message.Attachments.FindAll(attach => !attach.isEmbedded))
            {
                if (attachment.dataStream != null)
                {
                    attachment.dataStream.Seek(0, SeekOrigin.Begin);

                    var uploadedFileId = _apiHelper.UploadToCrm(attachment.dataStream, attachment.fileName,
                        attachment.contentType, contactEntity);

                    if (uploadedFileId != null)
                    {
                        fileIds.Add(uploadedFileId);
                    }
                }
                else
                {
                    var dataStore = _storageFactory.GetMailStorage(Tenant);

                    using var file = attachment.ToAttachmentStream(dataStore);

                    var uploadedFileId = _apiHelper.UploadToCrm(file.FileStream, file.FileName,
                        attachment.contentType, contactEntity);

                    if (uploadedFileId != null)
                    {
                        fileIds.Add(uploadedFileId);
                    }
                }
            }

            _apiHelper.AddToCrmHistory(message, contactEntity, fileIds);

            _log.InfoCrmLinkEngineAddRelationshipEvents(message.Id, contactEntity.Id);
        }
    }
}
