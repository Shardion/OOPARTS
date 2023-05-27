{
  description = "Nix build support for OOPARTS";
  inputs.nixpkgs.url = "nixpkgs/nixos-unstable";
  outputs = { self, nixpkgs }:
    let
      lastModifiedDate = self.lastModifiedDate or self.lastModified or "19700101";
      version = builtins.substring 0 8 lastModifiedDate;
      supportedSystems = [ "x86_64-linux" "aarch64-linux" ];
      forAllSystems = nixpkgs.lib.genAttrs supportedSystems;
      nixpkgsFor = forAllSystems (system: import nixpkgs { inherit system; overlays = [ self.overlay ]; });
    in

    {
      overlay = final: prev: {
        ooparts = with final; buildDotnetModule rec {
          name = "ooparts-${version}";

          src = builtins.path { path = ./.; name = "src"; };
          projectFile = "Shardion.OOPARTS/Shardion.OOPARTS.csproj";

          dotnet-sdk = dotnet-sdk_8;
          dotnet-runtime = dotnet-runtime_8;
          nugetDeps = "";

          buildInputs = with nixpkgsFor.${system}; [ openssl ];

          executables = [ "Shardion.OOPARTS" ];
          meta.mainProgram = "Shardion.OOPARTS";
        };
      };

      devShells = forAllSystems (system:
        {
          default = nixpkgsFor.${system}.mkShell rec {
            name = "default";
            packages = with nixpkgsFor.${system}; [
              omnisharp-roslyn
              dotnet-sdk_8
              openssl
            ];
          };
        }
      );

      packages = forAllSystems (system:
        {
          inherit (nixpkgsFor.${system}) ooparts;
        });
      defaultPackage = forAllSystems (system: self.packages.${system}.ooparts);
    };
}
