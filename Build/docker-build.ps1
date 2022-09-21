$repoRoot = (get-item $PSScriptRoot).Parent.FullName

docker build $repoRoot -t erzulie/mecon -f $repoRoot\Build\Dockerfile