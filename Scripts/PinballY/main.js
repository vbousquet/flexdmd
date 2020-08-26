/*
	Custom DMD screen script that shows informations on the selected game (more or less the same than PinballY) using custom medias (animated company logo, table title screen):
	
	- Shows image/video from the 'dmds/manufacturers' subfolder (hardcoded in the script)
	- Shows image/video from the 'dmds/titles' subfolder if they match the media name of the table
	- Shows highscores
	- Shows statistics
	- Can check for PinballY updates and display if any on the main screen (disabled by default)

	TODO:
	- GameName can not be modified for some reason to be understood
	- Missing Midway, Spooky Pinball, Jersey Jack Pinball logo animations
	- Missing animated logo for original tables
	- Move to FlexDMD API instead of UltraDMD when PinballY will fully marshall COM objects, and add more fancy animations
*/


// Check for a new version of PinballY on each launch (default is false to limit the load on PinballY's servers)
let checkPinballYUpdate = false;







// Check for new release of PinballY (taken from http://mjrnet.org/pinscape/downloads/PinballY/Help/UpdateCheckExample.html)
if (checkPinballYUpdate) {
	let request = new HttpRequest();
	request.open("GET", "http://mjrnet.org/pinscape/downloads/PinballY/VersionHistory.txt", true);
	request.send().then(reply =>
	{
		if (/^(\d\d)-(\d\d)-(\d\d\d\d) \(\d+\.\d+\.\d+ .+\)$/mi.test(reply))
		{
			let mm = +RegExp.$1 - 1, dd = +RegExp.$2, yyyy = +RegExp.$3;
			let onlineDate = new Date(Date.UTC(yyyy, mm, dd));
			if (onlineDate > systemInfo.version.buildDate)
				mainWindow.statusLines.upper.add("A new version of PinballY is available!");
		}
	}).catch(error => {
		logfile.log(
			"The Javascript version update checker ran into a problem!\nJavascript error: %s\nStack:\n%s",
			error.message, error.stack);
	});
}

// For debugging purposes
function getMethods(obj) {
  var result = [];
  for (var id in obj) {
    try {
      result.push(id + ": " + obj[id].toString() + " / frozen=" + Object.isFrozen(obj[id]) + " / sealed=" + Object.isSealed(obj[id]) + " / type=" + typeof(obj[id]));
    } catch (err) {
      result.push(id + ": inaccessible");
    }
  }
  return result;
}

Number.prototype.toHHMMSS = function () {
    var sec_num = this;
    var hours   = Math.floor(sec_num / 3600);
    var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
    var seconds = sec_num - (hours * 3600) - (minutes * 60);
    if (hours   < 10) {hours   = "0"+hours;}
    if (minutes < 10) {minutes = "0"+minutes;}
    if (seconds < 10) {seconds = "0"+seconds;}
    return hours+':'+minutes+':'+seconds;
}

Number.prototype.toDDHHMMSS = function () {
    var sec_num = this;
    var days   = Math.floor(sec_num / 86400);
    var hours   = Math.floor((sec_num - (days * 86400))/ 3600);
    var minutes = Math.floor((sec_num - (days * 86400) - (hours * 3600)) / 60);
    var seconds = sec_num - (days * 86400) - (hours * 3600) - (minutes * 60);
    if (hours   < 10) {hours   = "0"+hours;}
    if (minutes < 10) {minutes = "0"+minutes;}
    if (seconds < 10) {seconds = "0"+seconds;}
    return days+"d "+hours+':'+minutes+':'+seconds;
}

// Play a video, without looping, adapting to the actual length of the video
function queueVideo(filename, transitionIn, transitionOut, transitionMargin) {
	let video = dmd.NewVideo("", String(filename));
	let id = udmd.RegisterVideo(2, false, filename);
	udmd.DisplayScene00(id.toString(), "", 15, "", 15, transitionIn, video.Length * 1000 - transitionMargin, transitionOut);
}

// Handle DMD updates
let dmd = createAutomationObject("FlexDMD.FlexDMD");
let udmd = dmd.NewUltraDMD();
dmd.GameName = "";
dmd.Width = 128;
dmd.Height = 32;
dmd.Show = true;
dmd.Run = true;
var hiscores = {};
let info = null;
let shownInfo = null;
let loopCount = 0;
let fso = createAutomationObject("Scripting.FileSystemObject");
var manufacturers = {
	"Bally": ["./Scripts/dmds/manufacturers/bally.gif"],
	"Capcom": ["./Scripts/dmds/manufacturers/capcom.gif"],
	"Data East": ["./Scripts/dmds/manufacturers/dataeast-1.gif", "./Scripts/dmds/manufacturers/dataeast-2.gif"],
	"Gottlieb": ["./Scripts/dmds/manufacturers/gottlieb.gif"],
	"Midway": ["./Scripts/dmds/manufacturers/bally.gif"],
	"Premier": ["./Scripts/dmds/manufacturers/premier.gif"],
	"Sega": ["./Scripts/dmds/manufacturers/sega.gif"],
	"Stern": ["./Scripts/dmds/manufacturers/stern.gif"],
	"Williams": ["./Scripts/dmds/manufacturers/williams.gif"]
}
// logfile.log(getMethods(dmd).join("\n"));
function UpdateDMD() {
	if (info == null || (udmd.IsRendering() && info === shownInfo)) return;

	let rom = info.resolveROM();
	if (shownInfo == null || shownInfo.id != info.id) {
		logfile.log("> Update DMD for:");
		logfile.log("> rom: '".concat(rom.vpmRom, "'"));
		logfile.log("> manufacturer:", info.manufacturer);
		logfile.log("> title:", info.title);
		logfile.log("> year:", info.year);
		logfile.log("> Table type: ", info.tableType);
		logfile.log("> Highscore style: ", info.highScoreStyle);
		loopCount = 0;
	} else {
		loopCount++;
	}
	if (rom.vpmRom == null) {
		dmd.GameName = "";
	} else {
		// dmd.GameName = rom.vpmRom.toString();
	}
	udmd.CancelRendering();

	// Manufacturer
	let transitionMargin = (20 * 1000) / 60;
	if (info.manufacturer in manufacturers) {
		var medias = manufacturers[info.manufacturer];
		var media = medias[Math.floor(Math.random() * medias.length)];
		queueVideo(media, 10, 8, transitionMargin);
	} else if (info.manufacturer !== undefined) {
		udmd.DisplayScene00("FlexDMD.Resources.dmds.black.png", info.manufacturer, 15, "", 15, 10, 3000, 8);
	}
	
	// Title
	var hasTitle = false;
	if (info.mediaName != null) {
		var extensions = [".gif", ".avi", ".png"];
		for (var i = 0; i < extensions.length; i++) {
			if (fso.FileExists("./Scripts/dmds/titles/" + info.mediaName + extensions[i])) {
				queueVideo("./Scripts/dmds/titles/" + info.mediaName + extensions[i], 0, 8, transitionMargin);
				hasTitle = true;
				break;
			}
		}
	}
	if (!hasTitle) {
		var name = info.title.trim();
		var subname = "";
		if (name.indexOf('(') != -1) {
			var sep = info.title.indexOf('(');
			name = info.title.slice(0, sep - 1).trim();
		}
		if (name.length >= 16) {
			var split = 16;
			for (var i = 15; i > 0; i--) {
				if (name.charCodeAt(i) == 32) {
					subname = name.slice(i).trim();
					name = name.slice(0, i).trim();
					break;
				}
			}
		}
		udmd.DisplayScene00("FlexDMD.Resources.dmds.black.png", name, 15, subname, 15, 0, 5000, 8);
	}

	// Stats
	if (info.rating >= 0)
		udmd.DisplayScene00("FlexDMD.Resources.dmds.black.png", "Played " + info.playCount + " Rating " + info.rating, 15, "Play time: " + info.playTime.toHHMMSS(), 15, 10, 3000, 8);
	else
		udmd.DisplayScene00("FlexDMD.Resources.dmds.black.png", "Played " + info.playCount + " times", 15, "Playtime " + info.playTime.toHHMMSS(), 15, 10, 3000, 8);

	// Insert Coin (every 4 loops)
	if (((loopCount + 0) & 3) == 0) {
		queueVideo("./Scripts/dmds/misc/insertcoin.gif", 10, 14, 0);
		udmd.DisplayScene00("./Scripts/dmds/misc/insertcoin.gif", "", 15, "", 15, 10, 1399, 14);
		udmd.DisplayScene00("./Scripts/dmds/misc/insertcoin.gif", "", 15, "", 15, 14, 1399, 14);
	}

	// Global stats (every 4 loops)
	if (((loopCount + 1) & 3) == 0) {
		var totalCount = 0;
		var totalTime = 0;
		var nGames = gameList.getGameCount();
		for (var i = 0; i < nGames; i++) {
			var inf = gameList.getGame(i);
			totalCount += inf.playCount;
			totalTime += inf.playTime;
		}
		udmd.DisplayScene00("FlexDMD.Resources.dmds.black.png", "Total play count:" , 15, "" + totalCount, 15, 10, 1500, 8);
		udmd.DisplayScene00("FlexDMD.Resources.dmds.black.png", "Total play time:" , 15, "" + totalTime.toDDHHMMSS(), 15, 10, 1500, 8);
	}
	
	// Drink'n drive (every 4 loops)
	if (((loopCount + 2) & 3) == 0) {
		udmd.DisplayScene00("./Scripts/dmds/misc/drink'n drive.png", "", 15, "", 15, 10, 3000, 8);
	}
	
	// Highscores
	if (hiscores[info.id] != null) {
		udmd.ScrollingCredits("", hiscores[info.id].join("|"), 15, 14, 2800 + hiscores[info.id].length * 400, 14);
	}
	
	logfile.log("< Update DMD done");
	shownInfo = info;
}

UpdateDMD();
let updater = setInterval(UpdateDMD, 1000);

gameList.on("gameselect", event => {
	clearInterval(updater);
	info = event.game;
	udmd.CancelRendering();
	UpdateDMD();
	updater = setInterval(UpdateDMD, 1000);
});

gameList.on("highscoresready", event => {
	if (event.success && event.game != null) {
		logfile.log("> scores received");
		for (var i = 0; i < event.scores.length; i++) {
			event.scores[i] = event.scores[i].replace(/\u00FF/g, ',');
		}
		hiscores[event.game.id] = event.scores;
		if (shownInfo != null && event.game.id == shownInfo.id) {
			udmd.ScrollingCredits("", hiscores[shownInfo.id].join("|"), 15, 14, 2800 + hiscores[shownInfo.id].length * 400, 14);
		}
	}
});

mainWindow.on("prelaunch", event => {
	clearInterval(updater);
	udmd.CancelRendering();
	dmd.Run = false;
	logfile.log("> launch", event.commandId);
	logfile.log(getMethods(event).join("\n"));
});

mainWindow.on("postlaunch", event => {
	dmd.Run = true;
	updater = setInterval(UpdateDMD, 1000);
});