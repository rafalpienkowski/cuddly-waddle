# cuddly-waddl

# Requirements

- [dotnet 5](https://dotnet.microsoft.com/download/dotnet/5.0)
- [docker](https://www.docker.com/products/docker-desktop)

# Thinkg that could be implement:

- Better command validation, logging and metrics ex. by implementing [MediatR's behaviours](https://codewithmukesh.com/blog/mediatr-pipeline-behaviour/)
- Implementation of [the outbox pattern](https://microservices.io/patterns/data/transactional-outbox.html) for event bus

# Testing application

To run tests run in the root folder following command in your console:

```sh
dotnet test
```

Sample output:

```sh

  Determining projects to restore...
    All projects are up-to-date for restore.
      SMS.Domain -> /Users/rafal.pienkowski/Documents/github/cuddly-waddle/src/SMS.Domain/bin/Debug/net5.0/SMS.Domain.dll
        SMS.Service -> /Users/rafal.pienkowski/Documents/github/cuddly-waddle/src/SMS.Service/bin/Debug/net5.0/SMS.Service.dll
          SMS.Service.Tests -> /Users/rafal.pienkowski/Documents/github/cuddly-waddle/test/SMS.Service.Tests/bin/Debug/net5.0/SMS.Service.Tests.dll
          Test run for /Users/rafal.pienkowski/Documents/github/cuddly-waddle/test/SMS.Service.Tests/bin/Debug/net5.0/SMS.Service.Tests.dll (.NETCoreApp,Version=v5.0)
  Microsoft (R) Test Execution Command Line Tool Version 16.11.0
  Copyright (c) Microsoft Corporation.  All rights reserved.

  Starting test execution, please wait...
  A total of 1 test files matched the specified pattern.

  Passed!  - Failed:     0, Passed:    12, Skipped:     0, Total:    12, Duration: 35 ms
```


# Run the solution locally

- go to the `/src/SMS.App` directory
- run in your console the following command:

```sh
dotnet run
```

Sample output:

```sh

─ dotnet run                                                                                                                                                                                                                                                                                                            ─╯
  info: SMS.App.HostedServices.SmsCommandProducerHostedService[0]
        Starting SMS command producer
        Starting undelivered message sender
  info: Microsoft.Hosting.Lifetime[0]
        Application started. Press Ctrl+C to shut down.
  info: Microsoft.Hosting.Lifetime[0]
        Hosting environment: Production
  info: Microsoft.Hosting.Lifetime[0]
        Content root path: /Users/rafal.pienkowski/Documents/github/cuddly-waddle/src/SMS.App
  info: SMS.App.HostedServices.UndeliveredMessagesSenderHostedService[0]
        Start sending undelivered messages
  info: SMS.App.HostedServices.UndeliveredMessagesSenderHostedService[0]
        Send undelivered messages command
  info: SMS.App.HostedServices.SmsCommandProducerHostedService[0]
        New SMS command arrived
  info: SMS.Service.SendUndeliveredMessagesHandler[0]
        Gathering undelivered messages
  info: SMS.Service.SendSmsHandler[0]
        Sending SMS via external service
  info: SMS.Service.SendUndeliveredMessagesHandler[0]
        Found 0 undelivered messages
  info: SMS.Service.SendSmsHandler[0]
        Saving undelivered message for phone: +44 500 500 500
```

# Running application in docker environment

## Build docker image

- Run the following command in the root directory:

```sh
docker build -t sms_app:1.0 -f src/SMS.App/Dockerfile .
```

- Start the container by running the following command:

```
docker run -it sms_app:1.0
```

