# Signal Four Analytics - Next Generation

## Development

Refer to the [developer guide](https://geodevops.geoplan.ufl.edu/signal-four/S4-Analytics-Html5/wikis/developer-guide) to get started.

Before contributing any code, consult the following:
- [TypeScript Coding Guidelines](https://github.com/Microsoft/TypeScript/wiki/Coding-guidelines)
- [Angular2 Style Guide](https://angular.io/styleguide)
- [C# Coding Conventions](https://msdn.microsoft.com/en-us/library/ff926074.aspx)
- [Microsoft REST API Guidelines](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md)

## Builds

[GitLab CI](http://docs.gitlab.com/ce/ci/quick_start/README.html) is configured for this project:
- feature branches: build and test only
- master branch: build, test, and deploy to [dev environment](https://s4.geoplan.ufl.edu/analytics-html5-dev/)
- staging branch: build and deploy to [staging environment](https://s4.geoplan.ufl.edu/analytics-html5-staging/)
- demo branch: build and deploy to [demo environment](https://s4.geoplan.ufl.edu/analytics-html5-demo/)
- release branch: build and deploy to [release environment](https://s4.geoplan.ufl.edu/analytics-html5/)
