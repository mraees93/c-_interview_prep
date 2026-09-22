/*
2 separate objects on the heap, with 2 separate pointer addresses

Widely referred to as a "wrapper property" or an "exposed facade property". IReadOnlyCollection<T> is an interface. 
But because you are using a C# class Property block to wrap your private, raw List<T> inside a .AsReadOnly() shield, 
architects call this entire setup a Wrapper Property Pattern
*/

public class LegalCase
{
    // Holds mutable container reference on the heap
    private readonly List<string> _documentUrls = new(); //readonly only protects variable name, cant be re-assigned to new list in methods

    public string Title { get; private set; } = string.Empty;

    //Exposed interface contract blocks external code from calling .Clear() or .Add()
    public IReadOnlyCollection<string> DocumentUrls => _documentUrls.AsReadOnly(); //makes list container immutable to any code outside this class

    //Encapsulated method hook forces metadata validation before adding items.
    public void AttachDocument(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Document destination URL cannot be empty.");
        }

        _documentUrls.Add(url); // _documentUrls is my personal diary, only public methods in my class can access/mutate raw _documentUrls
    }
}