using Lib.Identity;
using Lib.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using S4Analytics.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace S4Analytics.Tests.Properties
{
    public class S4UserProfileStoreFixture : IDisposable
    {
        const string appName = "S4_Analytics";
        const string connStr =
            "User Id=s4_warehouse_dev;Password=crash418b;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=lime.geoplan.ufl.edu)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=oracle11g)));";
        const string membershipConnStr =
            "User Id=app_security_dev;Password=crash418b;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=lime.geoplan.ufl.edu)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=oracle11g)));";

        internal readonly OracleConnection Connection;
        internal readonly OracleConnection MembershipConnection;
        internal readonly S4UserProfileStore ProfileStore;

        public S4UserStore<S4IdentityUser<S4UserProfile>, S4UserProfile> UserStore { get; set; }
        public IPasswordHasher<S4IdentityUser<S4UserProfile>> PasswordHasher { get; set; }

        public S4UserProfileStoreFixture()
        {
            Connection = new OracleConnection(connStr);
            Connection.Open();

            MembershipConnection = new OracleConnection(membershipConnStr);
            MembershipConnection.Open();

            ProfileStore = new S4UserProfileStore(appName, Connection);
            UserStore = new S4UserStore<S4IdentityUser<S4UserProfile>, S4UserProfile>(appName, Connection, MembershipConnection, "", ProfileStore);
            PasswordHasher = new S4PasswordHasher<S4IdentityUser<S4UserProfile>>();
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }

    public class S4UserProfileStoreTests : IClassFixture<S4UserProfileStoreFixture>, IDisposable
    {
        private readonly OracleTransaction _trans;
        private readonly OracleTransaction _membershipTrans;
        private readonly S4UserStore<S4IdentityUser<S4UserProfile>, S4UserProfile> _userStore;
        private readonly IPasswordHasher<S4IdentityUser<S4UserProfile>> _passwordHasher;
        private readonly S4UserProfileStore _profileStore;

        public S4UserProfileStoreTests(S4UserProfileStoreFixture fixture)
        {
            _trans = fixture.Connection.BeginTransaction();
            _membershipTrans = fixture.MembershipConnection.BeginTransaction();
            _userStore = fixture.UserStore;
            _passwordHasher = fixture.PasswordHasher;
            _profileStore = fixture.ProfileStore;
        }

        public void Dispose()
        {
            _trans.Rollback();
            _membershipTrans.Rollback();
        }

        private string GenerateRandomUserName()
        {
            var rand = new Random();
            var num = rand.Next(10000, 99999);
            return $"xunit{num}";
        }

        private DateTime RemoveMilliseconds(DateTime dateTime)
        {
            return new DateTime(
                dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond),
                dateTime.Kind
            );
        }

        private async Task<S4IdentityUser<S4UserProfile>> CreateBasicUser(
            string userName,
            string email,
            string password,
            S4UserProfile profile,
            CancellationToken token)
        {
            var user = new S4IdentityUser<S4UserProfile>(userName);

            await _userStore.SetEmailAsync(user, email, token);

            var passwordHash = _passwordHasher.HashPassword(user, password);
            await _userStore.SetPasswordHashAsync(user, passwordHash, token);

            user.Profile = profile;

            await _userStore.CreateAsync(user, token);

            return user;
        }

        [Fact]
        public async void CreateBasicProfile()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var startDate = RemoveMilliseconds(DateTime.Now);
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                AccountExpirationDate = startDate.AddYears(1),
                AccountStartDate = startDate,
                EmailAddress = $"{userName}@ufl.edu",
                SuffixName = "Jr.",
                ForcePasswordChange = true,
                TimeLimitedAccount = true,
                CrashReportAccess = CrashReportAccess.After60Days,
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            Assert.NotNull(user.Profile);
            Assert.Equal("Dilbert", user.Profile.FirstName);
            Assert.Equal("Dewberry", user.Profile.LastName);
            Assert.Equal(startDate.AddYears(1), user.Profile.AccountExpirationDate);
            Assert.Equal(startDate, user.Profile.AccountStartDate);
            Assert.Equal($"{userName}@ufl.edu", user.Profile.EmailAddress);
            Assert.Equal("Jr.", user.Profile.SuffixName);
            Assert.True(user.Profile.ForcePasswordChange);
            Assert.True(user.Profile.TimeLimitedAccount);
            Assert.Equal(CrashReportAccess.After60Days, user.Profile.CrashReportAccess);
        }

        [Fact]
        public async void CreateProfileWithEmailMismatch()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            var badEmail = "dilbert@dewberry.com";
            await Assert.ThrowsAsync<Exception>(
                async () => await CreateBasicUser(userName, badEmail, "secret", profile, token));
        }

        [Fact]
        public async void UpdateBasicProfile()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var startDate = RemoveMilliseconds(DateTime.Now);
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                AccountExpirationDate = startDate.AddYears(1),
                AccountStartDate = startDate,
                EmailAddress = $"{userName}@ufl.edu",
                SuffixName = "Jr.",
                ForcePasswordChange = true,
                TimeLimitedAccount = true,
                CrashReportAccess = CrashReportAccess.After60Days,
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            user.Profile.FirstName = "Dogbert";
            user.Profile.LastName = "Doolittle";
            var newStartDate = startDate.AddDays(5);
            user.Profile.AccountStartDate = newStartDate;
            user.Profile.AccountExpirationDate = newStartDate.AddYears(1);
            user.Profile.EmailAddress = "dogbert@ufl.edu";
            user.Profile.SuffixName = "Sr.";
            user.Profile.ForcePasswordChange = false;
            user.Profile.TimeLimitedAccount = false;
            user.Profile.CrashReportAccess = CrashReportAccess.NoAccess;
            await _userStore.SetEmailAsync(user, user.Profile.EmailAddress, token);
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.NotNull(user.Profile);
            Assert.Equal("Dogbert", user.Profile.FirstName);
            Assert.Equal("Doolittle", user.Profile.LastName);
            Assert.Equal(newStartDate.AddYears(1), user.Profile.AccountExpirationDate);
            Assert.Equal(newStartDate, user.Profile.AccountStartDate);
            Assert.Equal("dogbert@ufl.edu", user.Profile.EmailAddress);
            Assert.Equal("Sr.", user.Profile.SuffixName);
            Assert.False(user.Profile.ForcePasswordChange);
            Assert.False(user.Profile.TimeLimitedAccount);
            Assert.Equal(CrashReportAccess.NoAccess, user.Profile.CrashReportAccess);
        }

        [Fact]
        public async void DeleteBasicProfile()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 }
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            Assert.NotNull(user.Profile);

            await _profileStore.DeleteProfileAsync(user, token);
            user = await _userStore.FindByNameAsync(userName, token);
            Assert.Null(user.Profile);
        }

        [Fact]
        public async void SetContractorCompany()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 }, // UF PD
                ContractorCompany = new Contractor { ContractorId = 69 } // UF
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            Assert.NotNull(user.Profile.ContractorCompany);
            Assert.Equal("University of Florida", user.Profile.ContractorCompany.ContractorName);
        }

        [Fact]
        public async void UpdateContractorCompany()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 }, // UF PD
                ContractorCompany = new Contractor { ContractorId = 69 } // UF
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            user.Profile.ContractorCompany = new Contractor { ContractorId = 73 }; // USF
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.Equal("University of South Florida", user.Profile.ContractorCompany.ContractorName);
        }

        [Fact]
        public async void UnsetContractorCompany()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 }, // UF PD
                ContractorCompany = new Contractor { ContractorId = 69 } // UF
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            user.Profile.ContractorCompany = null;
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.Null(user.Profile.ContractorCompany);
        }

        [Fact]
        public async void SetAgency()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            Assert.Equal("University of Florida Police Department", user.Profile.Agency.AgencyName);
        }

        [Fact]
        public async void UpdateAgency()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            user.Profile.Agency = new Agency() { AgencyId = 1340200 }; // FSU PD
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.Equal("Florida State University Police Department", user.Profile.Agency.AgencyName);
        }

        [Fact]
        public async void AddUserWithAgreement()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var signedDate = RemoveMilliseconds(DateTime.Now);
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            var userAgreement = new UserAgreement()
            {
                AgreementName = "User Agreement",
                SignedDate = signedDate,
                ExpirationDate = signedDate.AddYears(1)
            };
            profile.Agreements.Add(userAgreement);
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            Assert.NotEmpty(user.Profile.Agreements);
            userAgreement = user.Profile.Agreements[0];
            Assert.Equal("User Agreement", userAgreement.AgreementName);
            Assert.Equal(signedDate, userAgreement.SignedDate);
            Assert.Equal(signedDate.AddYears(1), userAgreement.ExpirationDate);
        }

        [Fact]
        public async void AddAgreement()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var signedDate = RemoveMilliseconds(DateTime.Now);
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            var userAgreement = new UserAgreement()
            {
                AgreementName = "User Agreement",
                SignedDate = signedDate,
                ExpirationDate = signedDate.AddYears(1)
            };
            user.Profile.Agreements.Add(userAgreement);
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.NotEmpty(user.Profile.Agreements);
            userAgreement = user.Profile.Agreements[0];
            Assert.Equal("User Agreement", userAgreement.AgreementName);
            Assert.Equal(signedDate, userAgreement.SignedDate);
            Assert.Equal(signedDate.AddYears(1), userAgreement.ExpirationDate);
        }

        [Fact]
        public async void UpdateAgreement()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var signedDate = RemoveMilliseconds(DateTime.Now);
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            var userAgreement = new UserAgreement()
            {
                AgreementName = "User Agreement",
                SignedDate = signedDate,
                ExpirationDate = signedDate.AddYears(1)
            };
            profile.Agreements.Add(userAgreement);
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            userAgreement = user.Profile.Agreements[0];
            var newSignedDate = signedDate.AddDays(5);
            userAgreement.SignedDate = newSignedDate;
            userAgreement.ExpirationDate = newSignedDate.AddYears(1);
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            userAgreement = user.Profile.Agreements[0];
            Assert.NotEqual(signedDate, userAgreement.SignedDate);
            Assert.NotEqual(signedDate.AddYears(1), userAgreement.ExpirationDate);
            Assert.Equal(newSignedDate, userAgreement.SignedDate);
            Assert.Equal(newSignedDate.AddYears(1), userAgreement.ExpirationDate);
        }

        [Fact]
        public async void AddCounties()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            profile.ViewableCounties.Add(new UserCounty()
            {
                CountyCode = 11, // Alachua
                CanEdit = true,
                CrashReportAccess = CrashReportAccess.Within60Days
            });
            profile.ViewableCounties.Add(new UserCounty()
            {
                CountyCode = 14, // Marion
                CanEdit = false,
                CrashReportAccess = CrashReportAccess.After60Days
            });
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            Assert.Equal(2, user.Profile.ViewableCounties.Count);
            Assert.Equal(1, user.Profile.EditableCounties.Count);
            var alachua = user.Profile.ViewableCounties.First(c => c.CountyName == "Alachua");
            var marion = user.Profile.ViewableCounties.First(c => c.CountyName == "Marion");
            Assert.Equal(true, alachua.CanEdit);
            Assert.Equal(CrashReportAccess.Within60Days, alachua.CrashReportAccess);
            Assert.Equal(false, marion.CanEdit);
            Assert.Equal(CrashReportAccess.After60Days, marion.CrashReportAccess);
            Assert.True(user.Profile.EditableCounties.Any(c => c.CountyName == "Alachua"));
        }

        [Fact]
        public async void UpdateCounties()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            profile.ViewableCounties.Add(new UserCounty()
            {
                CountyCode = 14, // Marion
                CanEdit = false,
                CrashReportAccess = CrashReportAccess.After60Days
            });
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            user.Profile.ViewableCounties.Add(new UserCounty()
            {
                CountyCode = 11, // Alachua
                CanEdit = true,
                CrashReportAccess = CrashReportAccess.Within60Days
            });
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.Equal(2, user.Profile.ViewableCounties.Count);
            Assert.Equal(1, user.Profile.EditableCounties.Count);
            var alachua = user.Profile.ViewableCounties.First(c => c.CountyName == "Alachua");
            var marion = user.Profile.ViewableCounties.First(c => c.CountyName == "Marion");
            Assert.Equal(true, alachua.CanEdit);
            Assert.Equal(CrashReportAccess.Within60Days, alachua.CrashReportAccess);
            Assert.Equal(false, marion.CanEdit);
            Assert.Equal(CrashReportAccess.After60Days, marion.CrashReportAccess);
            Assert.True(user.Profile.EditableCounties.Any(c => c.CountyName == "Alachua"));
        }

        [Fact]
        public async void RemoveCounty()
        {
            var token = new CancellationToken();
            var userName = GenerateRandomUserName();
            var profile = new S4UserProfile()
            {
                FirstName = "Dilbert",
                LastName = "Dewberry",
                EmailAddress = $"{userName}@ufl.edu",
                Agency = new Agency() { AgencyId = 1150200 } // UF PD
            };
            profile.ViewableCounties.Add(new UserCounty()
            {
                CountyCode = 11, // Alachua
                CanEdit = true,
                CrashReportAccess = CrashReportAccess.Within60Days
            });
            profile.ViewableCounties.Add(new UserCounty()
            {
                CountyCode = 14, // Marion
                CanEdit = false,
                CrashReportAccess = CrashReportAccess.After60Days
            });
            await CreateBasicUser(userName, profile.EmailAddress, "secret", profile, token);

            var user = await _userStore.FindByNameAsync(userName, token);
            user.Profile.ViewableCounties.Remove(user.Profile.ViewableCounties.Single(c => c.CountyName == "Alachua"));
            await _userStore.UpdateAsync(user, token);

            user = await _userStore.FindByNameAsync(userName, token);
            Assert.Equal(1, user.Profile.ViewableCounties.Count);
            Assert.Equal(0, user.Profile.EditableCounties.Count);
        }
    }
}
