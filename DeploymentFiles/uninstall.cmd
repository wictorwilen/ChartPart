
SET STSADM="c:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\12\bin\STSADM.EXE"
%STSADM% -o deactivatefeature -name ChartPart -url %1
%STSADM% -o execadmsvcjobs
%STSADM% -o retractsolution -name ChartPart.wsp -immediate -allcontenturls
%STSADM% -o execadmsvcjobs
%STSADM% -o deletesolution -name ChartPart.wsp
%STSADM% -o execadmsvcjobs
