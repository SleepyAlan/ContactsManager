using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ReceiveNewsLetters = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[,]
                {
                    { new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), "Denmark" },
                    { new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), "Argentina" },
                    { new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), "Indonesia" },
                    { new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), "Japan" },
                    { new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), "Canada" },
                    { new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), "China" },
                    { new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), "United States" },
                    { new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), "Mexico" },
                    { new Guid("d1ffeaab-42b4-4d53-9a4b-f0769518de15"), "South Korea" },
                    { new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), "Brazil" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Address", "CountryId", "DateOfBirth", "Email", "Gender", "PersonName", "ReceiveNewsLetters" },
                values: new object[,]
                {
                    { new Guid("003b945a-5e2e-45d1-a8b3-7c22a9b0e547"), "94 Rowland Terrace", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(2002, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bnoweak1k@lycos.com", "Female", "Blondell Noweak", true },
                    { new Guid("06ef9f56-278f-428f-8f3d-affdbd5cd957"), "1264 Blaine Circle", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1984, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "dkynoch22@spiegel.de", "Other", "Darice Kynoch", true },
                    { new Guid("08da6856-f6c9-4db1-ba7d-ff71a18a8ceb"), "7495 La Follette Circle", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(2002, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "bosipenko1y@rambler.ru", "Female", "Bryant Osipenko", false },
                    { new Guid("0a8431b3-b112-4cd0-9a64-42b31d0af48d"), "17430 Fremont Road", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1999, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "hsheaber1h@miitbeian.gov.cn", "Female", "Horacio Sheaber", true },
                    { new Guid("0ae8fc0a-312d-4127-86d4-75bc3d1a067f"), "99 Pepper Wood Way", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1999, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "tpearcy1v@disqus.com", "Male", "Theadora Pearcy", false },
                    { new Guid("0b51c539-5486-44c4-aaa0-2f474898584f"), "738 Larry Crossing", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(1990, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "menochsson2a@people.com.cn", "Male", "Monroe Enochsson", false },
                    { new Guid("0de220fd-2f46-4787-98c2-e4df0abdb9d0"), "82667 Canary Road", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1992, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "ibennison1c@vimeo.com", "Male", "Isador Bennison", false },
                    { new Guid("12312c9b-fdb2-4175-a2bc-8fbb913739a3"), "808 Talmadge Drive", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1979, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "skydde1o@w3.org", "Other", "Scotty Kydde", false },
                    { new Guid("1517d620-b274-4afa-a1b4-b10b17ad0c40"), "53 Arapahoe Place", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(1975, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "drazzell1z@blogs.com", "Female", "Dudley Razzell", false },
                    { new Guid("15e89259-6904-46e9-8399-2ea90bb1a6e9"), "6 Briar Crest Drive", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(2001, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "awotherspoonz@gravatar.com", "Female", "Ashli Wotherspoon", true },
                    { new Guid("1851d8e3-f1d9-410d-a892-27c779b694d4"), "5365 Melody Center", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1980, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "ablowin6@time.com", "Male", "Avrit Blowin", true },
                    { new Guid("1a7ac84a-6a40-4437-894d-299205e10c88"), "64 Rutledge Avenue", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1998, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "dbesnardeau2e@zdnet.com", "Male", "Denny Besnardeau", false },
                    { new Guid("1bb7f906-d0c3-4696-9f4a-99a8a599a243"), "608 Vernon Road", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1988, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "lryles18@acquirethisname.com", "Female", "Larina Ryles", true },
                    { new Guid("1d8d3d38-0458-425e-9e2d-a9df7a89f0e3"), "42 Welch Way", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1976, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "amcdonell1a@furl.net", "Female", "Ann McDonell", true },
                    { new Guid("21b0c918-7ef0-4568-b0fb-534ad747dfed"), "8 Schurz Court", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1977, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "ypepper2r@auda.org.au", "Other", "Yorke Pepper", true },
                    { new Guid("223d8b85-801a-4b2f-9eee-e23773a87164"), "061 Fulton Alley", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1988, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "meyer29@unblog.fr", "Other", "Margareta Eyer", false },
                    { new Guid("27f3e5d3-38b2-4610-ad5f-9cf636fb1a69"), "04164 Arkansas Junction", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(2008, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "fheading9@ning.com", "Other", "Feliks Heading", false },
                    { new Guid("2e48fbdb-3a12-46f4-acfc-8eee45ba63ae"), "17386 New Castle Crossing", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1974, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "rlook1e@ucsd.edu", "Male", "Rodge Look", false },
                    { new Guid("34331a83-ac65-4d1a-9e2b-b5023f34130d"), "054 Linden Crossing", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1994, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "mocorrigane1@jimdo.com", "Other", "Miguel O'Corrigane", false },
                    { new Guid("36a4fe40-3a7e-4c47-965f-51ecb3f53249"), "04 Straubel Street", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1991, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "smilbyb@slashdot.org", "Male", "Sibylla Milby", true },
                    { new Guid("36e8ccb0-8819-404c-b53d-bc7478f8942a"), "5 Walton Terrace", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(1994, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "tallett2@accuweather.com", "Other", "Timi Allett", true },
                    { new Guid("3d29a989-a1a5-46a0-8ef1-998a0c5ae234"), "956 Hallows Hill", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1993, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ifurbankv@dailymotion.com", "Male", "Imojean Furbank", true },
                    { new Guid("3f409025-a504-4948-abd0-d38a54e1ccd3"), "2183 Drewry Street", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(1984, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "rbryersi@webs.com", "Male", "Rozalin Bryers", true },
                    { new Guid("41037574-f2c0-4fac-9e36-dd4db4e6e3aa"), "3 Eagle Crest Lane", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(2001, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "fkenefordek@nhs.uk", "Male", "Fidelia Keneforde", true },
                    { new Guid("413d5231-2027-43e6-8a84-7acfab7db2ee"), "57 South Terrace", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1985, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "credingtons@netlog.com", "Other", "Cornall Redington", false },
                    { new Guid("43e58427-99fe-4c28-b863-e3ba5017df0e"), "1128 Hintze Junction", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1995, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "eyakoleva@webeden.co.uk", "Female", "Elsy Yakolev", false },
                    { new Guid("44045d2a-2a00-471a-bc33-c46fdee96496"), "0856 Nancy Park", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(2007, 11, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "sponte13@weather.com", "Female", "Sonni Ponte", true },
                    { new Guid("4467a045-340d-4b1f-a207-8785d2e7b115"), "782 Eliot Park", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1984, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "edanigelr@sakura.ne.jp", "Other", "Emalee Danigel", false },
                    { new Guid("46061096-e10f-4471-a45b-60fb784680ad"), "86 Daystar Avenue", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(2008, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "tseaking17@google.com.br", "Male", "Tate Seaking", true },
                    { new Guid("46411e3a-2395-45f4-9adf-b7f971d77d1b"), "5 Kennedy Alley", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1979, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "lsmail1g@answers.com", "Other", "Ludovico Smail", false },
                    { new Guid("494d69fa-576a-42ef-b6e2-4c77babf5cac"), "779 Clyde Gallagher Circle", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1985, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "spirson1q@dailymail.co.uk", "Male", "Sherry Pirson", true },
                    { new Guid("4bf687b2-4835-4539-bcea-ba2cc51d1313"), "39301 Red Cloud Hill", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(2004, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "cwickett24@skype.com", "Female", "Cointon Wickett", true },
                    { new Guid("4d1ff867-e3f0-4a0f-b30c-f4a1365d6d69"), "1 Gina Trail", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1997, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "wmumby7@simplemachines.org", "Female", "Whitman Mumby", false },
                    { new Guid("4f458f01-6fff-4198-bb0d-68de76b807a5"), "28950 Dakota Place", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1993, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "mslyman2p@cnn.com", "Female", "Marika Slyman", false },
                    { new Guid("501d78b1-d850-4f8b-b43d-fb6e573e7d08"), "0586 Eliot Street", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1990, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "esummerscales1t@mysql.com", "Other", "Elvis Summerscales", true },
                    { new Guid("56a505c1-9a20-4b99-b55c-bd6398d3c716"), "0 Spohn Lane", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1970, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "bemmisony@state.tx.us", "Female", "Bendicty Emmison", true },
                    { new Guid("576ac056-885f-4fc6-a696-e4deb7e01b15"), "81590 Browning Road", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1990, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "boutram10@deviantart.com", "Male", "Belita Outram", false },
                    { new Guid("591f6558-09d6-4951-84b7-75d53ed65fb0"), "5 Norway Maple Terrace", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1996, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "yhatrick2i@usda.gov", "Male", "Yehudit Hatrick", false },
                    { new Guid("5a666096-80bf-4a01-bd6f-f762cf8f02b7"), "910 Onsgard Place", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1989, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "gbernardottee@hugedomains.com", "Male", "Goldy Bernardotte", false },
                    { new Guid("5fbc438d-97d5-4d32-9f23-f49c7f7b47c2"), "703 Springs Center", new Guid("d1ffeaab-42b4-4d53-9a4b-f0769518de15"), new DateTime(1980, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "tyeulet0@naver.com", "Female", "Teresa Yeulet", false },
                    { new Guid("60784e99-efd0-4973-a452-e9e369f305fd"), "703 Arapahoe Avenue", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1991, 12, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "kphillott2h@usgs.gov", "Other", "Kerrin Phillott", true },
                    { new Guid("60a05370-ed63-4007-a7aa-91a29472c509"), "44194 Mosinee Pass", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1979, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "ddrakeleyc@xrea.com", "Other", "Delia Drakeley", true },
                    { new Guid("6197c7d4-2191-4fd4-a295-9b9d4299fe1f"), "314 Maple Trail", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1973, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "kbortolozzi1n@engadget.com", "Female", "Karl Bortolozzi", false },
                    { new Guid("63f7c4f8-8bf8-45e5-8035-d6b48bc4ea23"), "636 Briar Crest Crossing", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(2006, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "gofairy14@reuters.com", "Other", "Gerladina O'Fairy", true },
                    { new Guid("6442d4ea-492c-47c8-9a2a-67f2be654c77"), "4791 Delaware Drive", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1972, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "dmayoral2n@guardian.co.uk", "Male", "Davin Mayoral", false },
                    { new Guid("6631d37e-0a29-403d-b33d-d3be7a777b68"), "24586 Duke Center", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(2006, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "sfarnelld@fda.gov", "Other", "Sephira Farnell", false },
                    { new Guid("6883d011-070b-412a-b3f7-33b0339a95f3"), "6 Meadow Ridge Terrace", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1996, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "sespinay1i@naver.com", "Other", "Sheffy Espinay", false },
                    { new Guid("6d83276b-3cf3-4837-8e3f-94d3f51f483e"), "16053 Kropf Terrace", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1973, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "bliversidge16@marketwatch.com", "Female", "Brandon Liversidge", true },
                    { new Guid("6e7057ad-394f-4953-9b23-6292ace3ac51"), "03 Express Plaza", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1980, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "kgilogly2q@arstechnica.com", "Female", "Kare Gilogly", false },
                    { new Guid("71fa8111-6f72-4304-9281-5b8ef4b35d93"), "0853 Autumn Leaf Road", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1984, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "lbultitude23@hud.gov", "Female", "Lefty Bultitude", false },
                    { new Guid("744b75ea-079d-4aac-9fc4-abf31e33cdd1"), "8529 Ryan Point", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1992, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jmalicki1m@bbb.org", "Other", "Jodi Malicki", false },
                    { new Guid("74e6ca93-d953-4ba2-8795-717ca7aebbf3"), "13 Maywood Park", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1992, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "falfordel@ft.com", "Female", "Franciskus Alforde", false },
                    { new Guid("782a3ec5-edcc-4af3-9126-71ea4bbcc293"), "12120 Briar Crest Point", new Guid("d1ffeaab-42b4-4d53-9a4b-f0769518de15"), new DateTime(1986, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "acolgan15@hao123.com", "Male", "Adelaide Colgan", false },
                    { new Guid("78730aea-95b7-4b18-9a1b-e82a14d3ecca"), "07016 Fairfield Circle", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1973, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "cschrinel1x@sina.com.cn", "Male", "Carmita Schrinel", false },
                    { new Guid("7c25dbe7-709a-4dd8-9431-938720346f9d"), "704 Eggendart Road", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1979, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "jann1j@google.it", "Female", "Jo ann Pennicott", true },
                    { new Guid("7deaf702-34a1-4d25-a53c-54eddeb5f5a8"), "014 Gerald Parkway", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1980, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "abotterman21@blogtalkradio.com", "Male", "Alfredo Botterman", false },
                    { new Guid("7f8d8b3a-ab8a-4ace-be20-07d58ada4eac"), "7900 Donald Parkway", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1989, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "bmapstone11@unicef.org", "Other", "Beverley Mapstone", true },
                    { new Guid("836ac574-bdc2-4a34-84b9-6a134cc431fa"), "6 Stoughton Avenue", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1991, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "lpenkman26@hatena.ne.jp", "Male", "Lorne Penkman", true },
                    { new Guid("853694ea-27bd-46cf-8b83-f68fffccf0bd"), "9 Welch Place", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1996, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "msteels27@springer.com", "Other", "Maryann Steels", false },
                    { new Guid("8e0862d0-d520-41d0-ab5a-a12f5798d07b"), "7 Pennsylvania Center", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1987, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "lalves1r@yolasite.com", "Female", "Larine Alves", false },
                    { new Guid("92f9cb11-71c7-485a-86a0-bb61344d009d"), "337 Sauthoff Street", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1989, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "esonier2g@flavors.me", "Other", "Emery Sonier", true },
                    { new Guid("93eecadb-6289-4aa9-a174-c35b5ec0b54f"), "35 School Drive", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(2004, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "pdinjes2f@so-net.ne.jp", "Female", "Pepi Dinjes", false },
                    { new Guid("94c4f147-7283-43ce-9f0b-3aec459b7490"), "76 Grayhawk Place", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(2009, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "tdaniellott@umich.edu", "Male", "Terese Daniellot", true },
                    { new Guid("955c59cd-86df-4ebb-a2de-97e3ee160108"), "965 West Road", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1976, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "apook2k@etsy.com", "Female", "Alfie Pook", true },
                    { new Guid("9affbc03-fb72-4f9d-b1b0-ec971df8a656"), "26954 Prairie Rose Road", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(1978, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "aknox2j@mail.ru", "Other", "Archy Knox", true },
                    { new Guid("9b0814ad-507a-4002-a664-f5d941124222"), "96681 2nd Place", new Guid("d1ffeaab-42b4-4d53-9a4b-f0769518de15"), new DateTime(1989, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "dbirchner1u@google.com.hk", "Female", "Dot Birchner", true },
                    { new Guid("9c553f15-205e-4a47-9f57-e5ffec953c51"), "97904 Holy Cross Junction", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(2004, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "fbasketter25@spiegel.de", "Male", "Fina Basketter", false },
                    { new Guid("a172b64c-6e4d-4e00-83ba-62fbdabcb976"), "24 Maple Hill", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(2008, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "gluckman2d@mediafire.com", "Other", "Giralda Luckman", true },
                    { new Guid("a2455e53-8484-4b7b-958f-85fe44704fb9"), "85324 Corry Road", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1991, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "mschollm@histats.com", "Female", "Martie Scholl", true },
                    { new Guid("a2962597-a0e2-41a9-ab2f-1b0551fc8a6c"), "9 Cordelia Way", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1979, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "cbatchelar1f@canalblog.com", "Other", "Calvin Batchelar", false },
                    { new Guid("a39ba4ea-e4f8-4e4e-b3da-eed6902954f2"), "328 Buhler Crossing", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(1989, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "mmcmearty3@rediff.com", "Female", "Meaghan McMearty", true },
                    { new Guid("a487cff3-79bf-4cbb-b792-f58db382353f"), "92 Morningstar Park", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(2005, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "zbrafferton8@chron.com", "Male", "Zak Brafferton", false },
                    { new Guid("a4a68280-4d4f-47f6-9763-378782f192b5"), "816 Del Mar Drive", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1999, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "ediggens2m@sakura.ne.jp", "Male", "Estelle Diggens", true },
                    { new Guid("a4b079f4-778b-474e-b406-75f6a713ff03"), "7632 Artisan Avenue", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(2000, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jsurmann@canalblog.com", "Other", "Justinian Surman", false },
                    { new Guid("a7ed73f5-cfdf-4535-9acb-d338bf562042"), "20106 Pearson Plaza", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1980, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "hdannett1p@acquirethisname.com", "Other", "Howie Dannett", true },
                    { new Guid("b07e22bc-916e-4b47-9758-1731c9396095"), "206 Westend Court", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(1990, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "gbleythin1d@furl.net", "Female", "Gunar Bleythin", true },
                    { new Guid("b3110d6a-ed9b-4eb5-a023-fa6ec5f4566d"), "64524 Cambridge Trail", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(1992, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "tleybanq@netlog.com", "Other", "Tandie Leyban", true },
                    { new Guid("b70ad5c3-ea0b-47cc-88a5-4ce3382a323b"), "24157 Eastlawn Avenue", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(1993, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "ndunsire1w@prnewswire.com", "Female", "Nicola Dunsire", false },
                    { new Guid("b7bff5d7-cd05-4ad9-8cc9-2a15c7a82e34"), "05 Mifflin Alley", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1972, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "prawstronx@com.com", "Male", "Padraic Rawstron", true },
                    { new Guid("babf98d8-daae-47ec-94a0-e7d52e9584e2"), "729 Bobwhite Alley", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(2007, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "mbogies2c@alibaba.com", "Male", "Mahala Bogies", true },
                    { new Guid("c461ac6c-e940-4003-b61e-3eb005e964b5"), "27 Stuart Hill", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1978, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "cdilaweyw@liveinternet.ru", "Female", "Cosimo Dilawey", false },
                    { new Guid("c4a02176-78fa-466c-ada3-d16805180b00"), "4425 Trailsway Parkway", new Guid("d1ffeaab-42b4-4d53-9a4b-f0769518de15"), new DateTime(1986, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "jgietzh@comcast.net", "Male", "Jewel Gietz", false },
                    { new Guid("c5997d93-1c65-4e36-9aa9-1eee5d32ed4e"), "77979 Hoffman Center", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(1975, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "ghoferf@sitemeter.com", "Male", "Gavin Hofer", true },
                    { new Guid("c8942246-72ae-4c47-ad31-89606d5ae9ae"), "397 Hollow Ridge Way", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1998, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "pasquith1s@360.cn", "Male", "Peggi Asquith", true },
                    { new Guid("cd51029a-6580-4918-80ac-2a5e24a87dca"), "712 Alpine Way", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(2005, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "blabbeto@drupal.org", "Female", "Burlie Labbet", false },
                    { new Guid("cd72fa19-d230-4212-82e9-0fbede9d9d2d"), "0 Jackson Pass", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1987, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "emanjot1l@wikispaces.com", "Other", "Evonne Manjot", false },
                    { new Guid("cf13b971-b9b1-4342-b2aa-2770e761d4c9"), "9 Farmco Court", new Guid("d1ffeaab-42b4-4d53-9a4b-f0769518de15"), new DateTime(2001, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "twhitchurch20@adobe.com", "Other", "Tiffie Whitchurch", true },
                    { new Guid("cf8862be-6d9c-4664-91f5-d5db79a740fc"), "2374 Roth Junction", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1999, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "stassellg@reverbnation.com", "Male", "Sabine Tassell", true },
                    { new Guid("d5214250-a2a3-4210-bdb7-7620990556f9"), "90 Dottie Avenue", new Guid("6a42a633-541a-4130-ad50-3bbd2e29e9ae"), new DateTime(2002, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "barnoudu@jiathis.com", "Female", "Brietta Arnoud", false },
                    { new Guid("d6c52f78-85ab-4e27-bbe1-15fa31f575e3"), "15939 Laurel Drive", new Guid("6c5a9705-5494-4d30-a5fd-c867320f9399"), new DateTime(1979, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "gratke2o@samsung.com", "Male", "Gladi Ratke", false },
                    { new Guid("d6f18fbb-bdc6-4b53-9115-d4d25de1ca5d"), "53286 Kinsman Trail", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(1997, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "kbosman5@netvibes.com", "Other", "Koralle Bosman", false },
                    { new Guid("dbe12841-89b9-4897-a342-0e0d9acd08a7"), "549 Sage Center", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(1993, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "cocrevy1b@addtoany.com", "Female", "Cacilia O'Crevy", false },
                    { new Guid("dfecda3a-39d3-42cd-afcc-e0a3268782e4"), "8 Logan Hill", new Guid("0f9fdbd5-6bea-4873-aa5c-de758713d61d"), new DateTime(1983, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "kyoungman28@hexun.com", "Female", "Korrie Youngman", true },
                    { new Guid("e2f78415-9d95-4780-9278-c03ba2d11650"), "820 Beilfuss Point", new Guid("66b14595-ba52-4881-a43f-828cc9cbf925"), new DateTime(1989, 10, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "eannell2l@howstuffworks.com", "Male", "Elberta Annell", true },
                    { new Guid("e9eef8d1-83b3-41d7-8329-37bf1eada614"), "84 Sherman Center", new Guid("6d31a689-d784-4ab9-8b89-ded33f527262"), new DateTime(2001, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "dtunney2b@accuweather.com", "Male", "Doll Tunney", true },
                    { new Guid("f278297b-75c6-43c3-9ece-f3f6a9cf6b2e"), "4861 Superior Center", new Guid("3d9b0f61-d15c-4ac1-8395-feefa3d52327"), new DateTime(1983, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "teller12@creativecommons.org", "Female", "Trudi Eller", false },
                    { new Guid("f7c9649b-abb9-4e84-a856-3bbe2f5a51eb"), "16108 Pond Junction", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1991, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "tpichanp@ox.ac.uk", "Female", "Thatcher Pichan", false },
                    { new Guid("f7f16794-97cb-412b-a477-869751b1fd6b"), "155 Katie Circle", new Guid("8bac6c2a-ce92-4d31-ba1f-58154ea7839a"), new DateTime(1989, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jfennessy4@de.vu", "Male", "Jephthah Fennessy", true },
                    { new Guid("f9c378b3-2320-4108-a741-525fc7ec78a7"), "4 Namekagon Circle", new Guid("43a6ea02-4653-4bf4-a2ca-668c68560e09"), new DateTime(1996, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "baloigi19@unesco.org", "Female", "Bernie Aloigi", true },
                    { new Guid("ff3132a0-8b67-4089-90ed-af2ffbf3bebd"), "14820 Ronald Regan Park", new Guid("fb490384-9fce-4a7b-a026-ad07b747be8b"), new DateTime(1971, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "jdaalj@nsw.gov.au", "Female", "Jillene Daal", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
