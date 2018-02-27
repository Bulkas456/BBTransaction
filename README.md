If you need a simple and customizable transaction mechanism for a code methods which is able to continue processing steps after an unexpected crash or a power lost then this library is for you!
# Requirements:
The code is available for:
* .NET Framework 3.5 (no async await functionality)
* .NET Framework 4.5
* NET Core 1.0
# How it works
The library produces a transaction object which runs actions for a steps in a specific order. When an exception occurred or a transaction is cancelled then undo methods for a finished steps are processed. When transaction finish successfully then post actions are invoked.
Let's see an example:
First of all you need an idea for a transaction, i.e. safely write a binary data to a file:
1. Create a simple DTO for all necessary data to a file write operation:
```c#
public class FileWriteData
{
    public byte[] DataToWrite { get; set; }
    public string File { get; set; }
}
```
2. Create a transaction step id, it can be any type: enum, string, int etc.:
```c#
public enum WriteStepId
{
    CreateBackup,
    WriteData
}
```
3. Create a transaction instance with an appropriable settings using a factory:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options => 
{
    options.TransactionInfo.Name = "Example transaction";
});
```
4. Add steps to the created transaction:
```c#
transaction.Add(new TransactionStep<WriteStepId, FileWriteData>()
{
    Id = WriteStepId.CreateBackup,
    AsyncStepAction = CreateBackup,
    AsyncPostAction = CreateBackupPost
});
transaction.Add(new TransactionStep<WriteStepId, FileWriteData>()
{
    Id = WriteStepId.WriteData,
    AsyncStepAction = WriteData,
    AsyncUndoAction = WriteDataUndo
});

...

private async Task CreateBackup(FileWriteData data, IStepTransactionSessionInfo<WriteStepId> info)
{
    // Create a backup for an original file.
}

private async Task CreateBackupPost(FileWriteData data, IPostTransactionSessionInfo<WriteStepId> info)
{
    // Remove the created backup.
}

private async Task WriteData(FileWriteData data, IStepTransactionSessionInfo<WriteStepId> info)
{
    // Write the data from the 'data' instance to a specific file.
}

private async Task WriteDataUndo(FileWriteData data, IUndoTransactionSessionInfo<WriteStepId> info)
{
    // Remove the file and copy the file backup.
}
```
5. Run the transaction with data:
```c#
ITransactionResult<FileWriteData> result = await transaction.Run(settings => 
{
    settings.Mode = RunMode.Run;
    settings.Data = new FileWriteData()
    {
        DataToWrite = new byte[] { 0x01, 0x02 },
        File = "path to the file"
    };
});
```
6. Check the result:
```c#
switch (result.Result)
{
    case ResultType.Success:
        break;
    case ResultType.Cancelled:
        break;
    case ResultType.Failed:
        break;
    case ResultType.NoTransactionToRecover:
        break;
}
```
# Settings and features:
 ## Settings for a transaction creation
* Transaction logs forwarding: if an additional logs for a transaciton are needed then you can specify a transaction log forwarding: 
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()              
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.Name = "Example transaction";
    options.LoggerContext.DebugFormatAction = (format, parameters) => Debug.WriteLine(format, parameters);
    options.LoggerContext.ErrorFormatAction = (format, parameters) => Trace.WriteLine(string.Format(format, parameters));
    options.LoggerContext.ExecutionTimeLogAction = (time, format, parameters) => Debug.WriteLine(format, parameters);
});
```
or you can specify your own transaction logger:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.Name = "Example transaction";
    options.LoggerContext.Logger = new MyTransactionLogger();
});
```
* Transaction information settings
  - Transaction name: you can specify a transaction name which will be added to any logs:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.Name = "Example transaction";
});
```
  - Transaction time provider: you can set a time provider for each DateTime dependent transaction features:
```c#
 ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                        .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.GetCurrentTimeFunction = () => this.timeProvider.Now;
});
```
  - Session id: there is a possiblity to override a session id creation operation:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.SessionIdCreator = () => this.myGuidCreator.CreateGuid();
});
```
* Transaction state storage: when you specify a storage for a transaction state the transaction can be recoverable. It means that you will be able to continue a not finished transaction after an unexpected situation like an application crash or a power lost (see a point about recovering process).
 ```c#
 ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                        .Create<WriteStepId, FileWriteData>(options =>
 {
     options.TransactionStorageCreator = context => new MyStorage(context);
});
```
 ## Transaction run settings
* Specifying a transaction data. 
The data is injected to all transaction methods as the first parameter.
```c#
ITransactionResult<FileWriteData> result = await transaction.Run(settings => 
{
    settings.Data = new FileWriteData()
    {
        DataToWrite = new byte[] { 0x01, 0x02 },
        File = "path to the file"
    };
});
```
* Choose the run mode
There are three available modes for a transaction run which can be specified when invoking the Run method:
   - Run: this is a basic mode in which all steps are run in the defined order from the first one to the last one.
   - RecoverAndUndoAndRun: recovers the transaction (see Transaction recovering process), runs undo operations for completed steps and then starts the transaction from the first step to the last one. NOTE: If there is no session to recover the transaction is ended without run.
   - RecoverAndContinue: Recovers the transaction and runs not completed steps. NOTE: If there is no session to recover the transaction is ended without run.
```c#
transaction.Run(settings => 
{
    settings.Mode = RunMode.Run;
});
```
* Run callback
This is a method invoked when the transaction ends its work. When async await functionality is not available then this is the only way to get the transaciton result.
```c#
transaction.Run(settings => 
{
     settings.TransactionResultCallback = result => { };
});
```
* Other run settings
The property contains a few other settings as flags 
   - LogTimeExecutionForAllSteps: time execution for each step method will be written to log.
   - DontRecoverTransactionData: a transaction data wouldn't be recovered (see Transaction recovering process) and will be taken from the run settings.
```c#
transaction.Run(settings => 
{
     settings.Settings = TransactionSettings.DontRecoverTransactionData
                         | TransactionSettings.LogTimeExecutionForAllSteps;
});
```
# Executors for actions
# Transaction recovering process

# Build notes
You can remove the async await functionality from builds by specifying 'NOASYNC' in compilation symbols. 
