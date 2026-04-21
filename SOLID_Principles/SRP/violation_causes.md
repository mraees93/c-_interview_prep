High "Cognitive Load"
The Cause: The class is so large and does so many different things that it's difficult for a developer to hold the entire logic in their head.
The Sign: A file with hundreds (or thousands) of lines of code and many unrelated using statements at the top.
Frequent Merge Conflicts
The Cause: Multiple developers are constantly working on the same file because it contains logic for the UI, the database, and the business rules.
The Sign: In a team environment, you frequently have to resolve conflicts in the same "Manager" or "Service" class.
Low Testability/Fragility
The Cause: Because the logic is coupled, you cannot test the business rules without also triggering the database logic or the reporting logic.
The Sign: Your unit tests require massive "Setup" code (mocks for DB, mocks for File System, mocks for API) just to test one simple if statement.

mention that it makes the code highly maintainable. When a bug is found in the "PDF generation," you know exactly which file to open, and you aren't worried that changing the PDF logic will accidentally break the database saving logic.