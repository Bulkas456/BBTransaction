If you need a simple transaction mechanism for a code methods then this library is for you!
# How does it work?
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
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory().Create<WriteStepId, FileWriteData>(options => 
{
    options.TransactionInfo.Name = "Example transaction"
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
# Features:
* 
# Build notes
You can remove the async await functionality from builds by specifying 'NOASYNC' in compilation symbols. 
# Requirements:
The code is available for:
* .NET Framework 3.5 (no async await functionality)
* .NET Framework 4.5
* NET Core 1.0
