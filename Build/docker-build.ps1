$repoRoot = (get-item $PSScriptRoot).Parent.FullName

docker build $repoRoot -t elzik/mecon -f $repoRoot\Build\Dockerfile