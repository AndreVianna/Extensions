@echo off

if [%1]==[] goto USAGE
set target=%1

call Publish %target% Core\Core Core 9.0.0
call Publish %target% Core\Linq Linq 9.0.0
call Publish %target% Core\Graph Graph 9.0.0
call Publish %target% Data\Storage Data 9.0.0
call Publish %target% Data\File Data.File 9.0.0
call Publish %target% Utilities\ObjectDumper ObjectDumper 9.0.0
call Publish %target% Testing\TestUtilities TestUtilities 9.0.0
call Publish %target% Testing\FluentAssertions FluentAssertions 9.0.0
call Publish %target% Applications\Console Console 9.0.0
call Publish %target% Utilities\ValidationBuilder ValidationBuilder 9.0.0
call Publish %target% Cloud\Azure Azure 9.0.0
call Publish %target% Cloud\Http Http 9.0.0
call Publish %target% Utilities\Security Security 9.0.0
call Publish %target% AI\Core AI 9.0.0
call Publish %target% AI\Anthropic AI.Anthropic 9.0.0
call Publish %target% AI\OpenAI AI.OpenAI 9.0.0
goto :eof

:USAGE
echo Usage:
echo PublishAll ^<local^|remote^>
echo;
