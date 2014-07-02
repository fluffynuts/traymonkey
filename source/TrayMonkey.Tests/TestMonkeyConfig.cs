﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INI;
using PeanutButter.RandomGenerators;

namespace TrayMonkey.Tests
{
    [TestFixture]
    public class TestMonkeyConfig
    {
        [Test]
        public void Construct_WhenGivenNullINIFile_ShouldThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var ex = Assert.Throws<ArgumentNullException>(() => new MonkeyConfig(null));

            //---------------Test Result -----------------------
            Assert.AreEqual("iniFile", ex.ParamName);
            
        }

        [Test]
        public void Construct_WhenGivenINIFileWithNoSettings_ShouldHaveNoRules()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = new MonkeyConfig(iniFile);

            //---------------Test Result -----------------------
            Assert.IsNotNull(sut.Rules);
            Assert.IsFalse(sut.Rules.Any());
        }

        [Test]
        public void Construct_WhenGivenIniFileWithOnlyGlobalSettings_ShouldHaveNoRules()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            iniFile.AddSection("");
            var settingName = RandString();
            var settingValue = RandString();
            iniFile[""][settingName] = settingValue;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = CreateWith(iniFile);

            //---------------Test Result -----------------------
            Assert.IsNotNull(sut.Rules);
            Assert.IsFalse(sut.Rules.Any());
        }

        private static MonkeyConfig CreateWith(INIFile iniFile)
        {
            return new MonkeyConfig(iniFile);
        }

        private static string RandString()
        {
            return RandomValueGen.GetRandomAlphaString(1, 10);
        }


        [Test]
        public void Construct_WhenGivenIniFileWIthSettings_ShouldHaveRulesForSettings()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var name = RandString();
            var process = RandString();
            var onActivated = RandString();
            var onDeactivated = RandString();
            iniFile.AddSection(name);
            iniFile[name]["process"] = process;
            iniFile[name]["onactivated"] = onActivated;
            iniFile[name]["ondeactivated"] = onDeactivated;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = CreateWith(iniFile);

            //---------------Test Result -----------------------
            Assert.IsTrue(sut.Rules.Any());
            var rule = sut.Rules.First();
            Assert.AreEqual(process, rule.Process);
            Assert.AreEqual(onActivated, rule.OnActivated);
            Assert.AreEqual(onDeactivated, rule.OnDeactivated);
            Assert.AreEqual(name, rule.Name);
        }

        [Test]
        public void Construct_WhenGivenIniFile_ShouldSetHeartbeatFromGlobalSection()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var heartBeat = RandomValueGen.GetRandomInt(10, 1000);
            iniFile[""]["heartbeat"] = heartBeat.ToString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var config = CreateWith(iniFile);

            //---------------Test Result -----------------------
            Assert.AreEqual(heartBeat, config.Heartbeat);
        }

        [Test]
        public void Construct_WhenGivenIniFileWithNoHeartbeat_ShouldSetHeartbeatTo500()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            iniFile.AddSection("");

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var config = CreateWith(iniFile);

            //---------------Test Result -----------------------
            Assert.AreEqual(500, config.Heartbeat);
        }

    }
}
