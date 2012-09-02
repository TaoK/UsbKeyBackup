"c:\Program Files\Microsoft\ILMerge\ILMerge.exe" /t:winexe /out:%1NewUsbKeyBackup.exe %1UsbKeyBackup.exe %1EasyProgressDialog.dll %1es\UsbKeyBackup.resources.dll
del %1EasyProgressDialog.dll
del %1EasyProgressDialog.pdb
del %1UsbKeyBackup.exe
del %1UsbKeyBackup.pdb
del %1es\*.* /Q
rmdir %1es
copy %1UsbKeyBackup.exe.config %1NewUsbKeyBackup.exe.config
del %1UsbKeyBackup.exe.config
rem copy %1NewUsbKeyBackup.exe %1UsbKeyBackup.exe
rem copy %1NewUsbKeyBackup.pdb %1UsbKeyBackup.pdb
rem del %1NewUsbKeyBackup.exe
rem del %1NewUsbKeyBackup.pdb
