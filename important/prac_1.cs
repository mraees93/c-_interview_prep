// public async Task<string> GetCompressedDocumentJson(string docId)
// {
//     return await _compressionService.CompressAsync(docId);
// }
//this triggers thread pool starvation
// .Result and .Wait are sync methods