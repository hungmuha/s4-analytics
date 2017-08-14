# Signal Four Analytics - Next Generation

## Development

Refer to the [developer guide](https://geodevops.geoplan.ufl.edu/signal-four-gen2/S4-Analytics-Html5/wikis/developer-guide) to get started.

Before contributing any code, consult the following:
- [TypeScript Coding Guidelines](https://github.com/Microsoft/TypeScript/wiki/Coding-guidelines)
- [Angular2 Style Guide](https://angular.io/styleguide)
- [C# Coding Conventions](https://msdn.microsoft.com/en-us/library/ff926074.aspx)
- [SQL Style Guide](http://www.sqlstyle.guide/)
- [Microsoft REST API Guidelines](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md)

## Builds

[GitLab CI](http://docs.gitlab.com/ce/ci/quick_start/README.html) is configured for this project:
- feature branches: build and test only
- master branch: build, test, and deploy to https://s4.geoplan.ufl.edu/analytics-html5-dev/
- staging branch: build and deploy to https://s4.geoplan.ufl.edu/analytics-html5-staging/
- demo branch: build and deploy to https://s4.geoplan.ufl.edu/analytics-html5-demo/
- release branch: build and deploy to https://s4.geoplan.ufl.edu/analytics-html5/

## Related repositories

- [Lib.Identity](https://geodevops.geoplan.ufl.edu/signal-four-gen2/Lib.Identity)
- [S4-PBCAT](https://geodevops.geoplan.ufl.edu/signal-four-gen2/S4-PBCAT)
- [Oracle-Sync-ETL](https://geodevops.geoplan.ufl.edu/signal-four-gen2/Oracle-Sync-ETL)
