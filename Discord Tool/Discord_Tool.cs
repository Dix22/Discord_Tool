using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord_Tool
{
    internal class Program
    {
        static string botToken = "";
        static string guildId = "";
        const string configFile = "config.txt";

        static async Task Main(string[] args)
        {
            Console.Title = "Discord Tool - @deceep_";

            LoadOrCreateConfig();

            while (true)
            {
                Console.Clear();
                Banner();
                Menu();

                Console.Write("\nSelect an option: ");
                string option = Console.ReadLine();
                Console.WriteLine();

                switch (option)
                {
                    case "1":
                        await WebhookMessage();
                        break;
                    case "2":
                        await GetGuildInfo();
                        break;
                    case "3":
                        await GetMemberStatus();
                        break;
                    case "4":
                        await ListTextChannels();
                        break;
                    case "5":
                        await SendMessageToChannel();
                        break;
                    // case "6":
                    //    await ListRoles();
                    //    break;
                    case "7":
                        await KickMember();
                        break;
                    case "8":
                        await BanMember();
                        break;
                    case "9":
                        await ChangeNickname();
                        break;
                   // case "10":
                   //     await ViewBannedUsers();
                    //    break;
                    case "11":
                        await UnbanUser();
                        break;
                    case "12":
                        await CreateChannel();
                        break;
                    case "13":
                        await DeleteChannel();
                        break;
                    case "14":
                        await CreateRole();
                        break;
                 //   case "15":
                  //      await SearchMessagesByKeyword();
                   //     break;
                    case "16":
                        await ViewAuditLog();
                        break;
                    case "17":
                        await GetServerBoosts();
                        break;
                    case "18":
                        await ListServerInvites();
                        break;
                    case "00":
                        UpdateConfig();
                        break;
                    case "0":
                        Console.WriteLine("\nExiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void LoadOrCreateConfig()
        {
            if (File.Exists(configFile))
            {
                var lines = File.ReadAllLines(configFile);
                if (lines.Length >= 2)
                {
                    botToken = lines[0];
                    guildId = lines[1];

                    if (string.IsNullOrWhiteSpace(botToken) || string.IsNullOrWhiteSpace(guildId))
                    {
                        Console.WriteLine("Config file incomplete. Please enter data.");
                        RequestAndSaveConfig();
                    }
                }
                else
                {
                    Console.WriteLine("Config file incomplete. Please enter data.");
                    RequestAndSaveConfig();
                }
            }
            else
            {
                Console.WriteLine("Config file not found. Please enter data.");
                RequestAndSaveConfig();
            }
        }

        static void RequestAndSaveConfig()
        {
            Console.Write("Bot Token: ");
            botToken = Console.ReadLine();

            Console.Write("Guild ID: ");
            guildId = Console.ReadLine();

            SaveConfig();
        }

        static void SaveConfig()
        {
            File.WriteAllLines(configFile, new string[] { botToken, guildId });
            Console.WriteLine("Config saved.");
        }

        static void UpdateConfig()
        {
            Console.WriteLine("\n--- Update Config ---");
            Console.Write("New Bot Token (leave blank to keep current): ");
            string newToken = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newToken))
                botToken = newToken;

            Console.Write("New Guild ID (leave blank to keep current): ");
            string newGuild = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newGuild))
                guildId = newGuild;

            SaveConfig();
            Console.WriteLine("Config updated.");
        }

        static void Banner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"     ________  ________ _________  ________  ________  ___");
            Console.WriteLine(@"    |\   ___ \|\   ____\\___   ___\\   __  \|\   __  \|\  \");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(@"    \ \  \_|\ \ \  \___\|___ \  \_\ \  \|\  \ \  \|\  \ \  \");
            Console.WriteLine(@"     \ \  \ \\ \ \  \       \ \  \ \ \  \\\  \ \  \\\  \ \  \");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"      \ \  \_\\ \ \  \____   \ \  \ \ \  \\\  \ \  \\\  \ \  \____");
            Console.WriteLine(@"       \ \_______\ \_______\  \ \__\ \ \_______\ \_______\ \_______\");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"        \|_______|\|_______|   \|__|  \|_______|\|_______|\|_______|");
            Console.WriteLine("\n                                                       Discord Tool - @deceep_");
            Console.WriteLine(new string('-', 80));
        }

        static void Menu()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Options with an (M) mean they are under maintenance.");
            Console.WriteLine("╔═════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        MAIN MENU                        ║");
            Console.WriteLine("╠════════════════════════════╦════════════════════════════╣");

            Console.WriteLine("║ 1. Webhook Message         ║ 10. View Banned Users(M)   ║");
            Console.WriteLine("║ 2. View Guild Info         ║ 11. Unban User             ║");
            Console.WriteLine("║ 3. Check Member Status     ║ 12. Create Channel         ║");
            Console.WriteLine("║ 4. List Text Channels      ║ 13. Delete Channel         ║");
            Console.WriteLine("║ 5. Send Message to Channel ║ 14. Create Role            ║");
            Console.WriteLine("║ 6. List Roles(M)           ║ 15. Search Messages(M)     ║");
            Console.WriteLine("║ 7. Kick Member             ║ 16. View Audit log         ║");
            Console.WriteLine("║ 8. Ban Member              ║ 17. Server Boosts          ║");
            Console.WriteLine("║ 9. Change Nickname         ║ 18. Server Invites         ║");
            Console.WriteLine("║                            ║                            ║");
            Console.WriteLine("║ 0. Exit                    ║ 00. Update Token           ║");
            Console.WriteLine("╚════════════════════════════╩════════════════════════════╝");
        }

        static async Task WebhookMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔═══════════════════════════════════╗");
            Console.WriteLine("     ║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║        Send Webhook Message       ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚═══════════════════════════════════╝");

            Console.Write("\n=== > Enter Webhook URL: ");
            string webhook = Console.ReadLine();

            if (!Uri.IsWellFormedUriString(webhook, UriKind.Absolute))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid URL! Returning to main menu...");
                Console.ResetColor();
                return;
            }

            Console.Write("=== > Enter Message: ");
            string message = Console.ReadLine();

            string json = $"{{\"content\":\"{message}\"}}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(webhook, content);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("\nMessage sent successfully.");
                    else
                        Console.WriteLine($"\nError sending message: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        static async Task GetGuildInfo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔═══════════════════════════════════╗");
            Console.WriteLine("     ║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║         View Guild Info           ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚═══════════════════════════════════╝");


            Console.WriteLine($"\nFetching info for Guild ID: {guildId}\n");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    HttpResponseMessage response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}?with_counts=true");

                    string json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;

                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("\nServer Info:");


                            Console.WriteLine($"=== >  Name   : {root.GetProperty("name").GetString()}");
                            Console.WriteLine($"==== >  ID     : {root.GetProperty("id").GetString()}");

                            if (root.TryGetProperty("owner_id", out var ownerId))
                                Console.WriteLine($"=====>  Owner ID       : {ownerId.GetString()}");

                            if (root.TryGetProperty("region", out var region))
                                Console.WriteLine($"======>  Region         : {region.GetString()}");

                            if (root.TryGetProperty("verification_level", out var verification))
                                Console.WriteLine($"=======>  Verification  : {verification.GetInt32()}");

                            if (root.TryGetProperty("roles", out var roles))
                                Console.WriteLine($"========>  Roles Count   : {roles.GetArrayLength()}");

                            if (root.TryGetProperty("max_members", out var maxMembers))
                                Console.WriteLine($"=========>  Max Members   : {maxMembers.GetInt32()}");

                            if (root.TryGetProperty("icon", out var icon))
                            {
                                string iconId = icon.GetString();
                                string guildIdStr = root.GetProperty("id").GetString();
                                Console.WriteLine($"==========>  Icon URL      : https://cdn.discordapp.com/icons/{guildIdStr}/{iconId}.png");
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error {response.StatusCode}: {json}");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: {ex.Message}");
                Console.ResetColor();
            }
        }


        static async Task GetMemberStatus()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════╗");
            Console.WriteLine("║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("║         Check Member Status       ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("╚═══════════════════════════════════╝");


            Console.Write("=== >\nEnter User ID: ");
            string userId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nUser ID cannot be empty! Returning to main menu...");
                Console.ResetColor();
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"\nFetching member info for User ID: {userId}\n");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    HttpResponseMessage response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/members/{userId}");

                    string json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;
                            var user = root.GetProperty("user");

                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("\nMember Info:");
                            Console.WriteLine($"===>  Username        : {user.GetProperty("username").GetString()}#{user.GetProperty("discriminator").GetString()}");
                            Console.WriteLine($"====>  ID              : {user.GetProperty("id").GetString()}");

                            bool isBot = user.TryGetProperty("bot", out var botVal) && botVal.GetBoolean();
                            Console.WriteLine($"=====>  Is Bot          : {(isBot ? "Yes" : "No")}");

                            if (user.TryGetProperty("public_flags", out var flags))
                                Console.WriteLine($"======>  User Flags      : {flags.GetInt32()}");

                            if (root.TryGetProperty("nick", out var nickname) && nickname.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(nickname.GetString()))
                                Console.WriteLine($"=======>  Nickname        : {nickname.GetString()}");
                            else
                                Console.WriteLine($"=======>  Nickname        : None");

                            if (root.TryGetProperty("joined_at", out var joinedAt))
                                Console.WriteLine($"========>  Joined At       : {joinedAt.GetString()}");

                            if (root.TryGetProperty("roles", out var roles))
                                Console.WriteLine($"=========>  Role Count     : {roles.GetArrayLength()}");

                            if (root.TryGetProperty("mute", out var mute))
                                Console.WriteLine($"==========>  Muted         : {(mute.GetBoolean() ? "Yes" : "No")}");

                            if (root.TryGetProperty("deaf", out var deaf))
                                Console.WriteLine($"===========>  Deafened      : {(deaf.GetBoolean() ? "Yes" : "No")}");

                            if (user.TryGetProperty("avatar", out var avatar))
                            {
                                string uid = user.GetProperty("id").GetString();
                                string avatarId = avatar.GetString();
                                Console.WriteLine($"============>  Avatar URL    : https://cdn.discordapp.com/avatars/{uid}/{avatarId}.png");
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error {response.StatusCode}: {json}");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }
        static async Task ListTextChannels()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════╗");
            Console.WriteLine("║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("║        List Text Channels         ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("║                                   ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("╚═══════════════════════════════════╝");
            Console.WriteLine("=== > Listing text channels...\n");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/channels");
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            foreach (var channel in doc.RootElement.EnumerateArray())
                            {
                                if (channel.GetProperty("type").GetInt32() == 0)
                                {
                                    Console.WriteLine($"=== >> {channel.GetProperty("name").GetString()} (ID: {channel.GetProperty("id").GetString()}");
                                }
                            }
                        }
                    }
                    else Console.WriteLine("Failed to list channels.");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }
        static async Task SendMessageToChannel()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║        Send Message to Channel     ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");
            Console.WriteLine("                                           ");
            Console.Write("=== > Enter Channel ID: ");
            string channelId = Console.ReadLine();
            Console.WriteLine("===== >> Enter Message: ");
            string message = Console.ReadLine();

            var json = $"{{ \"content\": \"{message}\" }}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://discord.com/api/v10/channels/{channelId}/messages", content);
                    Console.WriteLine(response.IsSuccessStatusCode ? "$$$ Message sent.$$$" : "*** Failed to send message.***");

                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static async Task ListRoles()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║             List Roles             ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.WriteLine("=== > Listing roles...\n");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Autorization", $"Bot {botToken}");
                    var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/roles");
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            foreach (var role in doc.RootElement.EnumerateArray())
                            {
                                Console.WriteLine($"=== >> {role.GetProperty("name").GetString()} (ID: {role.GetProperty("id").GetString()})");
                            }
                        }
                    }
                    else Console.WriteLine("*** Failed to list roles. ***");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static async Task KickMember()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║             Kick Member            ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.WriteLine("=== > Enter User ID to kick: ");
            string userId = Console.ReadLine();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var response = await client.DeleteAsync($"https://discord.com/api/v10/guilds/{guildId}/members/{userId}");
                    Console.WriteLine(response.IsSuccessStatusCode ? "User kicked." : "Failed to kick user.");
                }
            }
            catch (Exception ex) { Console.WriteLine($"{ex.Message}"); }
        }

        static async Task BanMember()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║             Ban Member             ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.WriteLine("=== > Enter User ID to ban: ");
            string userId = Console.ReadLine();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var content = new StringContent("{}", Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"https://discord.com/api/v10/guilds/{guildId}/bans/{userId}", content);
                    Console.WriteLine(response.IsSuccessStatusCode ? "### User banned ###" : "*** Failed to ban user ***");
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }


        static async Task ChangeNickname()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║         Change Member Nickname     ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.Write("=== > Enter User ID: ");
            string userId = Console.ReadLine();
            Console.Write("=== >> Enter New Nickname: ");
            string newNick = Console.ReadLine();

            var json = $"{{ \"nick\": \"{newNick}\" }}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PatchAsync($"https://discord.com/api/v10/guilds/{guildId}/members/{userId}", content);
                    Console.WriteLine(response.IsSuccessStatusCode ? "Nickname changed." : "Failed to change nickname.");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static async Task ViewBannedUsers()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║         View Banned Users          ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/bans");
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var bans = JsonDocument.Parse(json).RootElement;
                        if (bans.GetArrayLength() > 0)
                        {
                            Console.WriteLine("No banned users found");
                            return;
                        }

                        foreach (var ban in bans.EnumerateArray())
                        {
                            var user = ban.GetProperty("user");
                            Console.WriteLine($"=== >  User: {user.GetProperty("username").GetString()}#{user.GetProperty("discriminator").GetString()} (ID: {user.GetProperty("id").GetString()})");
                            Console.WriteLine($"=== >  Reason: {(ban.TryGetProperty("reason", out var reason) ? reason.GetString() : "None")}");
                            Console.WriteLine(new string('-', 30));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get banned users: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        static async Task UnbanUser()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║           Unban User               ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.Write("=== > Enter User ID to unban: ");
            string userId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.WriteLine("User ID cannot be empty");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var response = await client.DeleteAsync($"https://discord.com/api/v10/guilds/{guildId}/bans/{userId}");

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("User unbanned successfully.");
                    else
                        Console.WriteLine($"Failed to unban user: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static async Task CreateChannel()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║           Create Channel           ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.Write("=== > Enter channel name: ");
            string channelName = Console.ReadLine();

            Console.Write("Enter Channel Type (0 = text, 2 = voice): ");
            string typeStr = Console.ReadLine();

            if (!int.TryParse(typeStr, out int type) || (type != 0 && type != 2))
            {
                Console.WriteLine("Invalid channel type.");
                return;
            }

            var json = $"{{\"name\":\"{channelName}\",\"type\":{type}}}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://discord.com/api/v10/guilds/{guildId}/channels", content);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("Channel created succesfully");

                    else
                        Console.WriteLine($"Failed to create channel: {response.StatusCode}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static async Task DeleteChannel()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║           Delete Channel           ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.Write("=== > Enter Channel ID to delete: ");
            string channelId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(channelId))
            {
                Console.WriteLine("Channel ID cannot be empty.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var response = await client.DeleteAsync($"https://discord.com/api/v10/channels/{channelId}");

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("Channel deleted successfully.");
                    else
                        Console.WriteLine($"Failed to delete channel: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        static async Task CreateRole()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║            Create Role             ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.Write("=== > Enter Role Name: ");
            string roleName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(roleName))
            {
                Console.WriteLine("Role name cannot be empty.");
                return;
            }

            var json = $"{{\"name\":\"{roleName}\"}}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://discord.com/api/v10/guilds/{guildId}/roles", content);

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine("Role created successfully." +
                            "Change role permissions from your Discord server");


                    else
                        Console.WriteLine($"Failed to create role: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        static async Task SearchMessagesByKeyword()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║      Search messages by keyword    ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            Console.Write("=== > Enter Channel ID: ");
            string channelId = Console.ReadLine();

            Console.Write("=== > Enter Keyword: ");
            string keyword = Console.ReadLine();

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                var response = await client.GetAsync($"https://discord.com/api/v10/channels/{channelId}/messages/search?content={Uri.EscapeDataString(keyword)}");

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to search messages.");
                    return;
                }

                var doc = JsonDocument.Parse(json);
                var results = doc.RootElement.GetProperty("messages");

                int count = 0;
                foreach (var group in results.EnumerateArray())
                {
                    foreach (var message in group.EnumerateArray())
                    {
                        string content = message.GetProperty("content").GetString();
                        string author = message.GetProperty("author").GetProperty("username").GetString();
                        Console.WriteLine($"{author}: {content}");
                        count++;
                    }
                }

                Console.WriteLine($"==== >> \nTotal messages found: {count}");
                doc.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        static async Task ViewAuditLog()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║           View Audit Log           ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/audit-logs");

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch audit logs.");
                    return;
                }

                var doc = JsonDocument.Parse(json);
                var entries = doc.RootElement.GetProperty("audit_log_entries");

                int count = 0;
                foreach (var entry in entries.EnumerateArray().Take(10))
                {
                    string actionType = entry.GetProperty("action_type").ToString();
                    string userId = entry.GetProperty("user_id").ToString();
                    Console.WriteLine($"Action Type: {actionType} | User ID: {userId}");
                    count++;
                }

                Console.WriteLine($"\nEntries displayed: {count}");
                doc.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        static async Task GetServerBoosts()
        {
            Console.Clear();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║           Server Boosts            ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}");

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to fetch server info.");
                    return;
                }

                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("premium_subscription_count", out var count))
                {
                    Console.WriteLine("=== > Active Boosts: " + count.GetInt32());
                }
                else
                {
                    Console.WriteLine("Could not retrieve boost count.");
                }

                doc.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static async Task ListServerInvites()
        {
            
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     ╔════════════════════════════════════╗");
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("     ║       Active server invites        ║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("     ║                                    ║");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("     ╚════════════════════════════════════╝");

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
                var response = await client.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/invites");

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to retrieve invites.");
                    return;
                }

                var doc = JsonDocument.Parse(json);
                var invites = doc.RootElement;

                int count = 0;
                foreach (var invite in invites.EnumerateArray())
                {
                    string code = invite.GetProperty("code").GetString();
                    string inviter = invite.GetProperty("inviter").GetProperty("username").GetString();
                    int uses = invite.GetProperty("uses").GetInt32();

                    Console.WriteLine($"=== > Invite: https://discord.gg/{code} | Created by: {inviter} | Uses: {uses}");
                    count++;
                }

                Console.WriteLine($"===== >>\nTotal invites: {count}");
                doc.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }



    }
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri) { Content = content };
            return await client.SendAsync(request);
        }
    }
}