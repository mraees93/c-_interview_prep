In REST API design, certain HTTP methods are classified as Idempotent. This means that making multiple identical requests to the server will leave the system in the exact same state as making a single request.
Which of the following lists contains only methods that are considered idempotent by standard HTTP rules?

GET, PUT, and DELETE

Methods like POST are non-idempotent. If you send a POST /loans request five times, the server will blindly execute the instruction five times, creating five completely separate loan applications on the database!



A user fills out a loan application form on Wonga's frontend interface. The React application sends a POST request to the backend .NET API. The server successfully validates the data, writes a brand-new loan application row into the database, and returns a success response containing the new record's ID.
According to standard RESTful design practices, which HTTP Status Code should the server return to the client?

201 Created

a 200 OK is a generic success code for reading or modifying data.
204 No Content means the request succeeded but the server is purposefully returning an entirely blank body (often used for simple DELETE or PUT actions). 302 Found is a redirection code, which would be an invalid resting state for a successful resource insertion.



One of the core constraints of the REST architectural style is that communication must be Stateless. 
What does statelessness mean for a backend .NET Web API service handling traffic from your client applications?

Every single request sent from a client app must contain all the context, data, and authentication tokens needed for the server to understand and process it. The server does not store any client context or session data on its own machines between requests.

Why Statelessness is Essential: If a server had to remember a user's login state locally in its own memory between calls (like the old days of server-side desktop sessions), you wouldn't be able to scale your application.

The Scaling Superpower: Because a REST API is stateless, you can run multiple copies of your Wonga backend API across ten different cloud servers in Cape Town. An incoming request from a user can hit Server #1, and their next click can route to Server #7. Since the request itself carries everything it needs (like a secure JWT authorization token), any server can immediately process it, making the application incredibly stable and easy to scale.



A user clicks a link inside your React application to view a loan statement, but they are not logged into the system. The application sends an API call anyway, and the backend .NET server blocks the request because it lacks a valid authentication token.
According to HTTP specifications, which Status Code Category should the server return to signal that the client made a bad or unauthorized request?

4xx Client Error codes (such as 401 Unauthorized)

The Rule of 4xx: Anytime a request fails because of something the client did—like passing invalid inputs, requesting a page that doesn't exist (404 Not Found), or failing to supply a security token (401 Unauthorized)—it falls squarely into the 4xx category. It tells the frontend app: "Fix the request on your side before trying again.
"The Rule of 5xx: You only use the 5xx category if the request was perfectly fine, but the server crashed internally while handling it (like a null pointer error or a database timeout).



When a client application interacts with a modern REST API, how does the client explicitly tell the backend server what format it wants the response data to be in (such as JSON or XML)?

By setting the standard HTTP Request Header called Accept to a value like application/json.

The Negotiation Process: A single API endpoint is technically capable of returning the exact same data model in multiple different formats (like JSON, XML, or even plain text). 
If a client sends an Accept header for a format the server doesn't support, the server will gracefully return a 406 Not Acceptable status code.



Your React frontend application runs locally on http://localhost:3000. It attempts to fetch data from your backend .NET API service running on a different port at http://localhost:5000. By default, the web browser blocks this request and prints a CORS error in your console.
What must you do to fix this common full-stack communication error?

You must configure the backend .NET API to explicitly allow requests originating from your frontend origin (http://localhost:3000) by setting up a CORS Policy in the backend middleware pipeline.

A CORS block is not a network connection failure or a server crash. It is a security feature enforced entirely by your web browser (like Chrome or Edge). The browser safely allows the network call to leave your app, but it stops your React frontend from reading the response unless the backend .NET API explicitly responds with an access header—specifically, Access-Control-Allow-Origin: http://localhost:3000. You must configure this policy directly in your .NET Startup.cs or Program.cs file.



When your React frontend application submits sensitive data—such as a user's password during login, or encrypted banking details for a loan payout request—where should this data be placed inside the HTTP request according to RESTful design standards?

Encapsulated inside the HTTP Request Body (Payload) as a formatted JSON object.



What type of REST routing parameter is being used by the {id} placeholder in this specific endpoint configuration?
[HttpGet("api/loans/{id}")]
public IActionResult GetLoanById(int id)
{
    // Fetch logic occurs here...
}

A Route/Path Parameter

When a variable placeholder is baked directly into the literal URL string path itself (like /api/loans/42), it is a Route/Path Parameter. It is used to identify a specific, singular resource in the database.

Query parameters are appended to the end of a URL after a question mark (like /api/loans?status=approved). They are used for optional filtering, sorting, or paging through groups of data, rather than identifying one specific object.



When updating an existing loan profile on a web server, what is the core behavioral difference between utilizing the PUT method versus the PATCH method?

PUT is used to completely replace an entire resource model with a brand-new payload copy, while PATCH is used to make partial modifications to only a few specific fields on an existing record.



What is the primary technical reason why JSON has become the industry standard for web browser data exchange?

JSON has a much more lightweight, compact syntax structure because it does not require repetitive closing tags. This significantly reduces data transfer sizes over the network and allows the browser's JavaScript engine to parse it natively and quickly.