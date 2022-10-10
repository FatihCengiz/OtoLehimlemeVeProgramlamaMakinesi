@ECHO.
@ECHO.
@ECHO. *****************************************************************************
@ECHO. ***                  		ROBIN          			         ***
@ECHO. ***                     PROGRAMLAMASI BASLATILIYOR                        ***
@ECHO. ***                                                                       ***
@ECHO. *****************************************************************************

@ECHO OFF
echo Starting Programming Application
cd "C:\Users\fatih.cengiz.ALPMERKEZ\Desktop\Test_Application\HEX-file"
dir /b /a-d > out.tmp
set /p E9.1.2=< out.tmp
del out.tmp
cd ..
echo.
echo.
echo Hex File name picked from the folder is: %E9.1.2%
echo.
@ECHO. -----------------------------------------------------------------------------
"C:\Users\fatih.cengiz.ALPMERKEZ\Desktop\Test_Application\PSoC_4_Programmer.exe" %E9.1.2%

