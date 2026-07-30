1. Client Requests: Law firms and legal corporate clients log into a web application to upload large batches of legal contracts and court transcripts (PDF, DOCX, and scanned images) for automated compliance scanning and risk analysis.

Write-heavy:
contract service => kafka => async workers 
    S3 blob => cassandra

1. user => DNS => load balancers (multiple to spread large amounts of traffic) => API Gateway (handle auth, security, rate limiting) if fail users cant talk to API's

(Producer Execution - whether to write to cassandra or postgreSQL)
2. c service (gets s3 link) => raw file to s3 blob => c service sends link(tokenized event containing metadata and s3 file pointer) to kafka => async consumer worker pulls lightweight message off designated Kafka partitions OR stream reads tiny JSON message string => async worker downloads raw pdf file from s3 blob => worker writes legal contracts OR court transcripts to cassandra.