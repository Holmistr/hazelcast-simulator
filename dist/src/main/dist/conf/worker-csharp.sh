#!/bin/bash

set -e

exec > worker.out
exec 2> worker.err

echo "Pokousim se nastartovat Csharp ${PWD}"

cd csharpworker

dotnet build
dotnet run