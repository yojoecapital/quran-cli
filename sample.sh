#!/bin/bash

set -e

# Check arguments
if [ $# -ne 1 ]; then
  echo "Usage: $0 <version>"
  exit 1
fi

VERSION=$1
TAG="v$VERSION"

# Update version in .csproj
echo "Updating version in .csproj files to $VERSION..."
find . -name "*.csproj" -exec sed -i "s|<Version>.*</Version>|<Version>${VERSION}</Version>|" {} +

# Build and publish
echo "Building and publishing project..."
dotnet publish -c Release -o $OUTPUT_DIR

# Check for existing tag/release
echo "Checking for existing tag/release..."
if git rev-parse "$TAG" >/dev/null 2>&1; then
  echo "Tag $TAG exists. Deleting existing tag..."
  git tag -d "$TAG"
  git push origin --delete "$TAG"
fi

# Push tag and commit version change
echo "Committing version update and creating new tag..."
git add .
git commit -m "Release version $VERSION"
git tag "$TAG"
git push origin --tags

# Publish GitHub release
if [ -f "$RELEASE_FILE" ]; then
  echo "Publishing GitHub release..."
  gh release create "$TAG" $OUTPUT_DIR/* --notes-file "$RELEASE_FILE"
else
  echo "Release file $RELEASE_FILE not found. Skipping release notes."
  gh release create "$TAG" $OUTPUT_DIR/*
fi

echo "Release $VERSION deployed successfully!"
