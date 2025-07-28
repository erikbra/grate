# Example of using grate in Automated Tests

A simplified example of one way to use `grate` in your automated tests.

This is certainly not the only way of integrating `grate` into your tests, but is a pattern that's been used in multiple companies with success.

⚠️ There's an implicit assumption that `grate` is installed on the system running the tests.  Ensure you have a 
step in your pipeline to install `grate` on the build server for CI (and that it's installed locally for development).