#!/bin/sh

if [[ -z "${TESTS_DIR}" ]]; then

echo "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" &&
echo "! ENVIROMENT SHOULD CONTAIN <TESTS_DIR> VARIABLE !" &&
echo "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"

else

dotnet test --collect:"XPlat Code Coverage" ${TESTS_DIR} &&\
TestResult=$(find . -name "coverage.cobertura.xml") &&\
dotnet /$(whoami)/.nuget/packages/reportgenerator/5.1.10/tools/net6.0/ReportGenerator.dll \
     -reports:${TestResult} \
     -targetdir:coveragereport \
     -reporttypes:Badges \
     -classfilters:-SpaceBattle.Runtime

fi
    
#rm -rf ${TestResult}