# MemberUnlock

[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/memberunlock/)

MemberUnlock is a Umbraco backoffice extension package which automatically unlock members after a period of time set in the appSettings of your web.config file.

## Getting Started

### Documentation

Members in Umbraco can get locked out when they have entered to many wrong passwords. The only 2 possible ways of removing this flag are:

- Manually set the member back to not locked out 
- When someone uses the password reset functionality they automatically get unlocked after updating their password

But not everyone uses this last functionality or doesn't have the time to always set the member manually back to not locked out, that's why we have created MemberUnlock.

MemberUnlock is a tiny package which unlock members. When? After a time which you can set in the web.config. You will find a new key in the appSettings section of your web.config file called: memberLockedOutInMinutes. The default value of this property is 10 which means a locked out member will be locked out for 10 minutes, after that MemberUnlock will unlock this member.

During the installation of this package we also create a Scheduled Task which you can find in the umbracoSettings.config file. This task will run every minute in order to unlock the members that may get unlocked.

If you want to view the history of these unlocked members by the MemberUnlock package, we alays add an entry in the log file like:

`MemberUnlock.Controllers.MemberUnlockApiController - Member 'test' has been unlocked automatically.`


### History Version

#### v1.0.0

Initial release of the MemberUnlock package.

## Contributors

* [Michaël Vanbrabandt](https://github.com/mivaweb)

## License

Copyright © 2018 Michaël Vanbrabandt

Licensed under the [MIT License](LICENSE.md)
