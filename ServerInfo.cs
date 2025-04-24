using System.IO;
using Life;
using Life.Network;
using Life.UI;
using Socket.Newtonsoft.Json;
using UnityEngine;

namespace ServerInfo
{
    public class ServerInfo : Plugin
    {
        private ServerInfoConfig config;

        public ServerInfo(IGameAPI api) : base(api) { }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            Debug.Log("\u001b[32m[By Spicy] ServerInfo ON\u001b[0m");

            new SChatCommand("/serverInfo", "", "/serverInfo", (player, args) =>
            {
                    if (player.account.adminLevel >= config.adminLevel) { MainPanel(player); }
            }).Register();

            LoadConfig();
        }

        public void LoadConfig()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "ServerInfoPlugin", "config.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<ServerInfoConfig>(json);
            }
            else
            {
                config = new ServerInfoConfig();
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, json);
            }
        }

        private void MainPanel(Player player)
        {
            UIPanel mainPanel = new UIPanel("mainPanel", UIPanel.PanelType.Tab);
            mainPanel.SetTitle($"<color={config.TitleColor}>Server Info</color>");

            if (player.account.adminLevel >= config.playersInfoAdminLevel) { mainPanel.AddTabLine("Players", _ => { player.ClosePanel(mainPanel); PlayersPanel(player); }); } else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            if (player.account.adminLevel >= config.serverInfoAdminLevel) { mainPanel.AddTabLine("Server", _ => { player.ClosePanel(mainPanel); ServerPanel(player); }); } else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            if (player.account.adminLevel >= config.staffsInfoAdminLevel) { mainPanel.AddTabLine("Staffs", _ => { player.ClosePanel(mainPanel); StaffsPanel(player); }); } else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            if (player.account.adminLevel >= config.bizsInfoAdminLevel) { mainPanel.AddTabLine("Bizs", _ => { player.ClosePanel(mainPanel); BizsPanel(player); }); } else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }

            mainPanel.AddButton($"<b><color={config.ConfirmButtonColor}>Sélectionner</color></b>", _ => mainPanel.SelectTab());
            mainPanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", _ => player.ClosePanel(mainPanel));
            player.ShowPanelUI(mainPanel);
        }

        private void PlayersPanel(Player player)
        {
            UIPanel playersPanel = new UIPanel("playersPanel", UIPanel.PanelType.Tab);
            playersPanel.SetTitle($"<color={config.TitleColor}>Players</color>");

            foreach (Player target in Nova.server.GetAllInGamePlayers())
            {
                if (target.IsAdminService) { playersPanel.AddTabLine($"Nom: <color=#446ad7>{target.FullName}</color> ID: {target.character.Id}", _ => { player.ClosePanel(playersPanel); InfoPlayer(player, target); }); }
                else if (target.serviceMetier) { playersPanel.AddTabLine($"Nom: <color=#c5bc29>{target.FullName}</color> ID: {target.character.Id}", _ => { player.ClosePanel(playersPanel); InfoPlayer(player, target); }); }
                else { playersPanel.AddTabLine($"Nom: <color=#e8e8e5>{target.FullName}</color> ID: {target.character.Id}", _ => { player.ClosePanel(playersPanel); InfoPlayer(player, target); }); }
            }

            playersPanel.AddButton($"<b><color={config.ConfirmButtonColor}>Sélectionner</color></b>", _ => playersPanel.SelectTab());
            playersPanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", _ => { player.ClosePanel(playersPanel); MainPanel(player); });
            playersPanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", _ => player.ClosePanel(playersPanel));
            player.ShowPanelUI(playersPanel);
        }

        private void InfoPlayer(Player player, Player target)
        {
            UIPanel infoPlPanel = new UIPanel("infoPlPanel", UIPanel.PanelType.Tab);
            infoPlPanel.SetTitle($"<color={config.TitleColor}>Info {target.FirstName}</color>");
            infoPlPanel.AddTabLine($"Prénom: {target.FirstName}", _ =>
            {
                if (player.account.AdminLevel >= config.changePlayerNameAdminLevel)
                {
                    UIPanel chFirstNamePanel = new UIPanel("chFirstNamePanel", UIPanel.PanelType.Input);
                    chFirstNamePanel.SetTitle($"<color={config.TitleColor}>Change Name</color>");
                    chFirstNamePanel.SetText("Entrez le nouveau prénom du joueur.");
                    chFirstNamePanel.SetInputPlaceholder("FirstName");
                    chFirstNamePanel.AddButton($"<b><color={config.ConfirmButtonColor}>Valider</color>", ui => { target.character.Firstname = chFirstNamePanel.inputText; });
                    chFirstNamePanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", ui => { player.ClosePanel(chFirstNamePanel); InfoPlayer(player, target); });
                    chFirstNamePanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", ui => player.ClosePanel(chFirstNamePanel));
                    player.ShowPanelUI(chFirstNamePanel);
                }
                else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            });
            infoPlPanel.AddTabLine($"Nom: {target.LastName}", _ =>
            {
                if (player.account.adminLevel >= config.changePlayerNameAdminLevel)
                {
                    UIPanel chLastNamePanel = new UIPanel("chLastNamePanel", UIPanel.PanelType.Input);
                    chLastNamePanel.SetTitle($"<color={config.TitleColor}>Change Name</color>");
                    chLastNamePanel.SetText("Entrez le nouveau prénom du joueur.");
                    chLastNamePanel.SetInputPlaceholder("FirstName");
                    chLastNamePanel.AddButton($"<b><color={config.ConfirmButtonColor}>Valider</color>", ui => { target.character.Lastname = chLastNamePanel.inputText; });
                    chLastNamePanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", ui => { player.ClosePanel(chLastNamePanel); InfoPlayer(player, target); });
                    chLastNamePanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", ui => player.ClosePanel(chLastNamePanel));
                    player.ShowPanelUI(chLastNamePanel);
                }
                else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            });
            infoPlPanel.AddTabLine($"Date de naissance: {target.character.Birthday}", _ =>
            {
                if (player.account.adminLevel >= config.changePlayerBirthdayDate)
                {
                    UIPanel chBirthdayDate = new UIPanel("chBirthdayDate", UIPanel.PanelType.Input);
                    chBirthdayDate.SetTitle($"<color={config.TitleColor}>Change Birthday</color>");
                    chBirthdayDate.SetText("Entrez la nouvelle date de naissance du joueur.");
                    chBirthdayDate.SetInputPlaceholder("XX/XX/XXXX");
                    chBirthdayDate.AddButton($"<b><color={config.ConfirmButtonColor}>Valider</color>", ui => { player.ClosePanel(chBirthdayDate); target.character.Birthday = chBirthdayDate.inputText; });
                    chBirthdayDate.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", ui => { player.ClosePanel(chBirthdayDate); InfoPlayer(player, target); });
                    chBirthdayDate.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", ui => player.ClosePanel(chBirthdayDate));
                    player.ShowPanelUI(chBirthdayDate);
                }
                else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            });
            infoPlPanel.AddTabLine($"ID: {target.character.Id}", _ => { });
            infoPlPanel.AddTabLine($"Steam ID: {target.steamId}", _ => { });
            string adminServiceEnabled;

            if (target.serviceAdmin) { adminServiceEnabled = "<color=#3c9c27>Activé</color>"; }
            else { adminServiceEnabled = "<color=#a71616>Désactivé</color>"; }

            infoPlPanel.AddTabLine($"Service Admin: {adminServiceEnabled}", _ =>
            {
                if (player.account.adminLevel >= config.changePlayerAdminServiceStatus)
                {
                    UIPanel disableAdminService = new UIPanel("disableAdminService", UIPanel.PanelType.Text);
                    disableAdminService.SetTitle($"<color={config.TitleColor}>Change Admin Service</color>");
                    disableAdminService.SetText($"Etes-vous sûr de vouloir désactiver / activer le service Admin de {target.FullName} ?");
                    disableAdminService.AddButton($"<b><color={config.ConfirmButtonColor}>Valider</color>", ui => { player.ClosePanel(disableAdminService); if (target.serviceAdmin == true) { target.IsAdminService = false; } else { target.IsAdminService = true; } });
                    disableAdminService.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", ui => { player.ClosePanel(disableAdminService); InfoPlayer(player, target); });
                    disableAdminService.AddButton($"<b><color={config.CancelButtonColor}>Annuler</color></b>", ui => player.ClosePanel(disableAdminService));
                    player.ShowPanelUI(disableAdminService);
                }
                else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            });
            infoPlPanel.AddTabLine($"Admin Level: {target.account.adminLevel}", _ =>
            {
                if (player.account.adminLevel >= config.changePlayerAdminLevel)
                {
                    UIPanel changeAdminLevel = new UIPanel("changeAdminLevel", UIPanel.PanelType.Input);
                    changeAdminLevel.SetTitle($"<color={config.TitleColor}>Change Admin Level</color>");
                    changeAdminLevel.SetText($"Entrez le niveau d'Admin que vous souhaitez attribuer à {target.FullName}");
                    changeAdminLevel.SetInputPlaceholder("Admin Level");
                    if (int.TryParse(changeAdminLevel.inputText, out int newAdminLevel)) { } else { return; }
                    changeAdminLevel.AddButton($"<b><color={config.ConfirmButtonColor}>Valider</color>", ui => { player.ClosePanel(changeAdminLevel); target.account.adminLevel = newAdminLevel; });
                    changeAdminLevel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", ui => { player.ClosePanel(changeAdminLevel); InfoPlayer(player, target); });
                    changeAdminLevel.AddButton($"<b><color={config.CancelButtonColor}>Annuler</color></b>", ui => player.ClosePanel(changeAdminLevel));
                    player.ShowPanelUI(changeAdminLevel);
                }
                else { player.Notify("ERREUR", "Vous n'avez pas les permissions nécessaires !", NotificationManager.Type.Warning); }
            });
            // infoPlPanel.AddTabLine($"Voir Inventaire", _ => player.setup.TargetOpenPlayerInventory((uint)target.character.Id));
            infoPlPanel.AddButton($"<b><color={config.ConfirmButtonColor}>Sélectionner</color></b>", _ => infoPlPanel.SelectTab());
            infoPlPanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", _ => { player.ClosePanel(infoPlPanel); MainPanel(player); });
            infoPlPanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", _ => player.ClosePanel(infoPlPanel));
            player.ShowPanelUI(infoPlPanel);
        }

        private void ServerPanel(Player player)
        {
            UIPanel serverPanel = new UIPanel("serverPanel", UIPanel.PanelType.Text);
            serverPanel.SetTitle($"<color={config.TitleColor}>Server Panel</color>");
            serverPanel.SetText("<size=15><b>SOON !</b></size>");
            serverPanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", _ => { player.ClosePanel(serverPanel); MainPanel(player); });
            serverPanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", _ => player.ClosePanel(serverPanel));
            player.ShowPanelUI(serverPanel);
        }

        private void StaffsPanel(Player player)
        {
            UIPanel staffsPanel = new UIPanel("staffsPanel", UIPanel.PanelType.Text);
            staffsPanel.SetTitle($"<color={config.TitleColor}>Staffs Panel</color>");
            staffsPanel.SetText("<size=15><b>SOON !</b></size>");
            staffsPanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", _ => { player.ClosePanel(staffsPanel); MainPanel(player); });
            staffsPanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", _ => player.ClosePanel(staffsPanel));
            player.ShowPanelUI(staffsPanel);
        }

        private void BizsPanel(Player player)
        {
            UIPanel bizsPanel = new UIPanel("bizsPanel", UIPanel.PanelType.Text);
            bizsPanel.SetTitle($"<color={config.TitleColor}>Bizs Panel</color>");
            bizsPanel.SetText("<size=15><b>SOON !</b></size>");
            bizsPanel.AddButton($"<b><color={config.BackButtonColor}>Retour</color></b>", _ => { player.ClosePanel(bizsPanel); MainPanel(player); });
            bizsPanel.AddButton($"<b><color={config.CancelButtonColor}>Fermer</color></b>", _ => player.ClosePanel(bizsPanel));
            player.ShowPanelUI(bizsPanel);
        }
    }

    public class ServerInfoConfig
    {
        public string TitleColor { get; set; } = "#2ec4b9";
        public string ConfirmButtonColor { get; set; } = "#3c9c27";
        public string CancelButtonColor { get; set; } = "#a71616";
        public string BackButtonColor { get; set; } = "#824af1";
        public int adminLevel { get; set; } = 5;
        public int playersInfoAdminLevel { get; set; } = 5;
        public int serverInfoAdminLevel { get; set; } = 5;
        public int staffsInfoAdminLevel { get; set; } = 5;
        public int bizsInfoAdminLevel { get; set; } = 5;
        public int changePlayerNameAdminLevel { get; set; } = 5;
        public int changePlayerAdminServiceStatus { get; set; } = 5;
        public int changePlayerAdminLevel { get; set; } = 5;
        public int changePlayerBirthdayDate { get; set; } = 5;
    }
}
