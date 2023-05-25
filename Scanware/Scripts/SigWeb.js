var tmr;

var resetIsSupported = false;
var SigWeb_1_6_4_0_IsInstalled = false; //SigWeb 1.6.4.0 and above add the Reset() and GetSigWebVersion functions
var SigWeb_1_7_0_0_IsInstalled = false; //SigWeb 1.7.0.0 and above add the GetDaysUntilCertificateExpires() function

window.onload = function () {
    if (IsSigWebInstalled()) {
        var sigWebVer = "";
        try {
            sigWebVer = GetSigWebVersion();
        } catch (err) { console.log("Unable to get SigWeb Version: " + err.message) }

        if (sigWebVer != "") {
            try {
                SigWeb_1_7_0_0_IsInstalled = isSigWeb_1_7_0_0_Installed(sigWebVer);
            } catch (err) { console.log(err.message) };
            //if SigWeb 1.7.0.0 is installed, then enable corresponding functionality
            if (SigWeb_1_7_0_0_IsInstalled) {

                resetIsSupported = true;
                try {
                    var daysUntilCertExpires = GetDaysUntilCertificateExpires();
                    document.getElementById("daysUntilExpElement").innerHTML = "SigWeb Certificate expires in " + daysUntilCertExpires + " days.";
                } catch (err) { console.log(err.message) };
                var note = document.getElementById("sigWebVrsnNote");
                note.innerHTML = "SigWeb 1.7.0 installed";
            } else {
                try {
                    SigWeb_1_6_4_0_IsInstalled = isSigWeb_1_6_4_0_Installed(sigWebVer);
                    //if SigWeb 1.6.4.0 is installed, then enable corresponding functionality						
                } catch (err) { console.log(err.message) };
                if (SigWeb_1_6_4_0_IsInstalled) {
                    resetIsSupported = true;
                    var sigweb_link = document.createElement("a");
                    sigweb_link.href = "https://www.topazsystems.com/software/sigweb.exe";
                    sigweb_link.innerHTML = "https://www.topazsystems.com/software/sigweb.exe";

                    var note = document.getElementById("sigWebVrsnNote");
                    note.innerHTML = "SigWeb 1.6.4 is installed. Install the newer version of SigWeb from the following link: ";
                    note.appendChild(sigweb_link);
                } else {
                    var sigweb_link = document.createElement("a");
                    sigweb_link.href = "https://www.topazsystems.com/software/sigweb.exe";
                    sigweb_link.innerHTML = "https://www.topazsystems.com/software/sigweb.exe";

                    var note = document.getElementById("sigWebVrsnNote");
                    note.innerHTML = "A newer version of SigWeb is available. Please uninstall the currently installed version of SigWeb and then install the new version of SigWeb from the following link: ";
                    note.appendChild(sigweb_link);
                }
            }
        } else {
            //Older version of SigWeb installed that does not support retrieving the version of SigWeb (Version 1.6.0.2 and older)
            var sigweb_link = document.createElement("a");
            sigweb_link.href = "https://www.topazsystems.com/software/sigweb.exe";
            sigweb_link.innerHTML = "https://www.topazsystems.com/software/sigweb.exe";

            var note = document.getElementById("sigWebVrsnNote");
            note.innerHTML = "A newer version of SigWeb is available. Please uninstall the currently installed version of SigWeb and then install the new version of SigWeb from the following link: ";
            note.appendChild(sigweb_link);
        }
    }
    else {
			//alert("Unable to communicate with SigWeb. Please confirm that SigWeb is installed and running on this PC.");
		}
}

function isSigWeb_1_6_4_0_Installed(sigWebVer) {
    var minSigWebVersionResetSupport = "1.6.4.0";

    if (isOlderSigWebVersionInstalled(minSigWebVersionResetSupport, sigWebVer)) {
        console.log("SigWeb version 1.6.4.0 or higher not installed.");
        return false;
    }
    return true;
}

function isSigWeb_1_7_0_0_Installed(sigWebVer) {
    var minSigWebVersionGetDaysUntilCertificateExpiresSupport = "1.7.0.0";

    if (isOlderSigWebVersionInstalled(minSigWebVersionGetDaysUntilCertificateExpiresSupport, sigWebVer)) {
        console.log("SigWeb version 1.7.0.0 or higher not installed.");
        return false;
    }
    return true;
}

function isOlderSigWebVersionInstalled(cmprVer, sigWebVer) {
    return isOlderVersion(cmprVer, sigWebVer);
}

function isOlderVersion(oldVer, newVer) {
    const oldParts = oldVer.split('.')
    const newParts = newVer.split('.')
    for (var i = 0; i < newParts.length; i++) {
        const a = parseInt(newParts[i]) || 0
        const b = parseInt(oldParts[i]) || 0
        if (a < b) return true
        if (a > b) return false
    }
    return false;
}

function onSign() {
    if (IsSigWebInstalled()) {
        var ctx = document.getElementById('canvsig').getContext('2d');
        SetDisplayXSize(500);
        SetDisplayYSize(100);
        SetTabletState(0, tmr);
        SetJustifyMode(0);
        ClearTablet();
        if (tmr == null) {
            tmr = SetTabletState(1, ctx, 50);
        }
        else {
            SetTabletState(0, tmr);
            tmr = null;
            tmr = SetTabletState(1, ctx, 50);
        }
    } else {
        alert("Unable to communicate with SigWeb. Please confirm that SigWeb is installed and running on this PC.");
    }
}

function onDone() {
    if (NumberOfTabletPoints() == 0) {
        alert("Please sign before continuing");
    }
    else {
        SetTabletState(0, tmr);
        //RETURN TOPAZ-FORMAT SIGSTRING
        SetSigCompressionMode(1);
        //document.FORM1.sigStringData.value = GetSigString();
        //this returns the signature in Topaz's own format, with biometric information

        //RETURN BMP BYTE ARRAY CONVERTED TO BASE64 STRING
        SetImageXSize(500);
        SetImageYSize(100);
        SetImagePenWidth(5);

        GetSigImageB64(SigImageCallback);
    }
}

function SigImageCallback(str) {
    document.getElementById('MySignature').value = str;
}

function endDemo() {
    ClearTablet();
    SetTabletState(0, tmr);
}

function close() {
    if (resetIsSupported) {
        Reset();
    } else {
        endDemo();
    }
}

//Perform the following actions on
//	1. Browser Closure
//	2. Tab Closure
//	3. Tab Refresh
window.onbeforeunload = function (evt) {
    close();
    clearInterval(tmr);
    evt.preventDefault(); //For Firefox, needed for browser closure
};