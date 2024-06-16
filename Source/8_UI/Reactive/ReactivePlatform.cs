﻿using BeatLeader.UI.Reactive.Yoga;
using IPA.Logging;
using YGLogLevel = BeatLeader.UI.Reactive.Yoga.YogaNative.YGLogLevel;

namespace BeatLeader.UI.Reactive {
    internal static class ReactivePlatform {
        public static void Init() {
            InitYoga();
        }

        #region Yoga

        private static void InitYoga() {
            //0 means that the point values won't be rounded
            YogaConfig.Default.SetPointScaleFactor(0f);
            YogaNative.YGBindingsSetLogger(LogYogaMessage);
        }

        private static void LogYogaMessage(string message, YGLogLevel level) {
            var ipaLogLevel = ConvertYogaLogLevel(level);
            Plugin.Log.Log(ipaLogLevel, message);

            static Logger.Level ConvertYogaLogLevel(YGLogLevel logLevel) {
                return logLevel switch {
                    YGLogLevel.YGLogLevelError => Logger.Level.Error,
                    YGLogLevel.YGLogLevelWarn => Logger.Level.Warning,
                    YGLogLevel.YGLogLevelInfo => Logger.Level.Info,
                    YGLogLevel.YGLogLevelDebug => Logger.Level.Debug,
                    YGLogLevel.YGLogLevelVerbose => Logger.Level.Trace,
                    YGLogLevel.YGLogLevelFatal => Logger.Level.Critical,
                    _ => Logger.Level.None
                };
            }
        }

        #endregion
    }
}