# Init

This sets up the infrastructure in the azure subscription which is used to set up the test infrastructure on each test run.

It creates a resource group, and a service principal with full access to that resource group, to make it easier to control access.

This folder is typically only run once-ish.