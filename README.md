# MemberUnlock

[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/memberunlock/)

MemberUnlock is a Umbraco backoffice extension package which automatically unlock members after a period of time set in the appSettings of your web.config file.

## Getting Started

### Documentation

Members in Umbraco can get locked out when they have entered to many wrong passwords. The only 2 possible ways of removing this flag are:

- Manually set the member back to not locked out 
- When someone uses the password reset functionality they automatically get unlocked after updating their password

But not everyone uses this last functionality or doesn't have the time to always manually set the member back to not locked out, that's way we created MemberUnlock.

This package adds a new key into the appSettings of your web.config file called: memberLockedOutInMinutes. The default value of this property is 10 which means that locked out members stay locked out for at least 10 minutes.

It also creates a scheduled task which runs every minute in order to check all locked out members and there locked out timeframe. Once the locked out tmeframe is expired, MemberUnlock will unlock them.


### History Version

#### v1.0.0

Initial release of the MemberUnlock package.

## Contributors

* [Michaël Vanbrabandt](https://github.com/mivaweb)

## License

Copyright © 2018 Michaël Vanbrabandt

Licensed under the [MIT License](LICENSE.md)
