using System;
using System.Collections.Generic;
using HarmonyLib;
using Lunaris;
using UnityEngine;

namespace ErenshorCommandHistory
{
    internal static class PluginIdentity
    {
        public const string Guid = "com.github.Jhenox.erenshorcommandhistory";
        public const string Version = "0.1.0";
    }

    [LunarisPlugin("Erenshor Command History", PluginIdentity.Version, "Jhenox",
        "Recall previously entered slash commands with Up and Down arrows")]
    [LunarisPermission(LunarisPermission.Harmony)]
    public class Plugin : LunarisPlugin
    {
        private Harmony harmony;

        private void Awake()
        {
            try
            {
                harmony = new Harmony(PluginIdentity.Guid);
                harmony.PatchAll(typeof(Plugin).Assembly);
                Logging.LogInfo("Erenshor Command History v" + PluginIdentity.Version + " loaded.");
            }
            catch (Exception ex)
            {
                Logging.LogError("Command History initialization failed: " + ex);
                if (harmony != null) harmony.UnpatchSelf();
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }
        }
    }

    internal static class CommandHistory
    {
        private const int MaxEntries = 50;
        private static readonly List<string> Entries = new List<string>();
        private static int position = -1; // -1 means not browsing
        private static string draft = "";

        internal static void Record(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { Reset(); return; }
            string command = value.Trim();
            // Store commands, but not /whisper messages or regular chat.
            if (command.StartsWith("/", StringComparison.Ordinal) &&
                !IsPrivateMessage(command) &&
                (Entries.Count == 0 || Entries[Entries.Count - 1] != command))
            {
                Entries.Add(command);
                if (Entries.Count > MaxEntries) Entries.RemoveAt(0);
            }
            Reset();
        }

        private static bool IsPrivateMessage(string command)
        {
            return command.Equals("/w", StringComparison.OrdinalIgnoreCase) ||
                   command.StartsWith("/w ", StringComparison.OrdinalIgnoreCase) ||
                   command.Equals("/whisper", StringComparison.OrdinalIgnoreCase) ||
                   command.StartsWith("/whisper ", StringComparison.OrdinalIgnoreCase);
        }

        internal static void Reset()
        {
            position = -1;
            draft = "";
        }

        internal static void Navigate(TypeText chat)
        {
            if (chat == null || chat.InputBox == null || !chat.InputBox.activeSelf || chat.typed == null)
                return;
            if (Entries.Count == 0) return;

            bool up = Input.GetKeyDown(KeyCode.UpArrow);
            bool down = Input.GetKeyDown(KeyCode.DownArrow);
            if (!up && !down) return;

            if (up)
            {
                if (position == -1)
                {
                    draft = chat.typed.text;
                    position = Entries.Count - 1;
                }
                else if (position > 0)
                {
                    position--;
                }
                chat.typed.text = Entries[position];
            }
            else if (position != -1)
            {
                position++;
                if (position >= Entries.Count)
                {
                    position = -1;
                    chat.typed.text = draft;
                    draft = "";
                }
                else
                {
                    chat.typed.text = Entries[position];
                }
            }
        }
    }

    // CheckInput runs for both built-in commands and commands handled by other mods.
    // Capture before the game clears the text field.
    [HarmonyPatch(typeof(TypeText), "CheckInput")]
    internal static class RecordCommandPatch
    {
        [HarmonyPrefix]
        private static void Prefix(TypeText __instance)
        {
            if (__instance != null && __instance.typed != null)
                CommandHistory.Record(__instance.typed.text);
        }
    }

    // The game's Update handles typing and opening the chat field. Navigate afterward
    // so the recalled command is not overwritten by normal input processing.
    [HarmonyPatch(typeof(TypeText), "Update")]
    internal static class NavigateHistoryPatch
    {
        [HarmonyPostfix]
        private static void Postfix(TypeText __instance)
        {
            CommandHistory.Navigate(__instance);
        }
    }

    [HarmonyPatch(typeof(TypeText), "CloseInputBox")]
    internal static class CloseChatPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            CommandHistory.Reset();
        }
    }
}
