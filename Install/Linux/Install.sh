#!/bin/bash

INSTALL_DIR="${INSTALL_DIR:-"/usr/local/bin"}"

# Get relevant .NET Runtime Identifier from architecture
ARCH=$(uname -m)
case $ARCH in
    x86_64|ia64) RID=linux-x64 ;;
    *) echo "$ARCH is not a supported architecture. Raise an issue to support additional architectures. https://github.com/elzik/mecon/issues"; exit 1;;
esac

# prepare the download URL
GITHUB_LATEST_VERSION=$(curl -L -s -H 'Accept: application/json' https://github.com/elzik/mecon/releases/latest | sed -e 's/.*"tag_name":"\([^"]*\)".*/\1/')
GITHUB_FILE="mecon-${GITHUB_LATEST_VERSION/}-${RID}.tar.gz"
GITHUB_URL="https://github.com/elzik/mecon/releases/download/${GITHUB_LATEST_VERSION}/${GITHUB_FILE}"

# install/update the local binary
curl -L -o mecon.tar.gz "$GITHUB_URL"
file mecon.tar.gz
tar xzvf mecon.tar.gz mecon
install -Dm 755 mecon -t "$INSTALL_DIR"
[[ ":$PATH:" != *":$INSTALL_DIR:"* ]] && PATH="${PATH}:$INSTALL_DIR"
rm mecon mecon.tar.gz