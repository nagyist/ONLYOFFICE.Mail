﻿namespace ASC.Mail.Aggregator.Service.Console;

[Singletone]
public class ConsoleParser
{
    readonly string[] _args;

    public ConsoleParser(string[] args)
    {
        _args = args;
    }

    public ConsoleParameters GetParsedParameters()
    {
        var _consoleParameters = new ConsoleParameters();

        if (_args.Any())
        {
            CommandLine.Parser.Default.ParseArguments<ConsoleParameters>(_args)
                .WithParsed(param => _consoleParameters = param)
                .WithNotParsed(errs =>
                {
                    var helpText = @"Bad command line parameters.";
                    System.Console.Error.Write(helpText);
                });
        }

        return _consoleParameters;
    }
}
