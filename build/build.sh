#!/usr/bin/env bash

set -e

# version
major="2"
minor="3"

basepath="${PWD}"
artifacts="${basepath}/artifacts"
nuget_server="https://www.nuget.org/api/v2/package"
revision=${GITHUB_REF:="alpha"}
buildnumber=${GITHUB_RUN_NUMBER:=1}
version="${major}.${minor}.${buildnumber}"

export VERSION=${version}

if [ -d ${artifacts} ]; then  
  rm -R ${artifacts}
fi

echo "Base path: ${basepath}"
echo "Artifacts: ${artifacts}"
echo "Revision:  ${revision}"
echo "Build #:   ${buildnumber}"
echo "Version:   ${version}"
echo "Secret:    ${NUGET_AUTH_TOKEN}"
echo "BUILD:     ${BUILD_NUMBER}"
echo "BRANCH:    ${BRANCH}"

dotnet nuget locals all --clear

dotnet restore

dotnet build -c Release

if [ "${1}" == "deploy" ]; then
  dotnet pack -c Release -o ${artifacts} /p:PackageVersion=${version} --no-dependencies

  dotnet nuget push "${artifacts}/Simple*.nupkg" -s ${nuget_server} -k ${NUGET_AUTH_TOKEN}
fi
