
SET STSADM="c:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\12\bin\STSADM.EXE"
%STSADM% -o addsolution -filename ChartPart.wsp
%STSADM% -o execadmsvcjobs
%STSADM% -o deploysolution -name ChartPart.wsp -immediate -allowgacdeployment -allcontenturls
%STSADM% -o execadmsvcjobs
%STSADM% -o activatefeature -name ChartPart -url %1
%STSADM% -o execadmsvcjobs