#!/bin/bash

find . -iname "*.fbx.*meta" | awk '{ printf("sed -e \"s/ meshCompression: 0/ meshCompression: 2/g\" \"%s\" > \"%s.bak\"; mv \"%s.bak\" \"%s\" \n", $0, $0, $0, $0) }' | bash
find . -iname "*.fbx.*meta" | awk '{ printf("sed -e \"s/ meshCompression: 1/ meshCompression: 2/g\" \"%s\" > \"%s.bak\"; mv \"%s.bak\" \"%s\" \n", $0, $0, $0, $0) }' | bash
