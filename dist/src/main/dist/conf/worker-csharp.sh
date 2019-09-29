#!/bin/bash

set -e

exec > worker.out
exec 2> worker.err

python worker.py