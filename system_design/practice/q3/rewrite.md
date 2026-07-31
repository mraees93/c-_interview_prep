1. Client Requests: Law firms and legal corporate clients log into a web application to upload large batches of legal contracts and court transcripts (PDF, DOCX, and scanned images) for automated compliance scanning and risk analysis.

Write-heavy:
contract service => kafka => async workers 
    S3 blob => cassandra

1. user => DNS => load balancers (multiple to spread large amounts of traffic) => API Gateway (handle auth, security, rate limiting) if fail users cant talk to API's

(Producer Execution - whether to write to cassandra or postgreSQL)
2. c service (gets s3 link) => raw file to s3 blob => c service sends link(tokenized event containing metadata and s3 file pointer) to kafka => async consumer worker pulls lightweight message off designated Kafka partitions OR stream reads tiny JSON message string => async worker downloads raw pdf file from s3 blob => worker writes legal contracts OR court transcripts to cassandra.



2. Third-Party Integration: The system must securely send the extracted text to external, specialized AI legal-compliance engines and government trademark/patent database APIs for validation. These external API calls are highly throttled, prone to intermittent timeouts, and take anywhere from 15 seconds to 3 minutes to return results.

third party service => kafka => tasks goes in message queue => async workers securely send the extracted text to external api's 
        => notification service gets notification via websocket that api validation completed => sends completion message to ui or email

[Third-Party Service] ➔ [Kafka: Requests] ➔ [Workers] ➔ [Kafka: Completions] ➔ [Notification Service] ➔ [Client Browser]
