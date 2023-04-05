cask "grate" do
    version "1.4.0"
    sha256 "091b03b0d2a7c6b0146eeca6f05ffdc2cf337d5c67cf79557b2ce47554f6975c"

    url "https://github.com/erikbra/grate/releases/download/#{version}/grate-osx-x64-self-contained-#{version}.zip"
    name "grate"
    desc "grate - the SQL scripts migration runner"
    homepage "https://erikbra.github.io/grate/"

    binary "grate"

end
