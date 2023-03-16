DOTNET_TOOLS	:= $(HOME)/.dotnet/tools

setup-services: install execute-migrations

install:
	dotnet tool install --verbosity quiet --tool-path $(DOTNET_TOOLS) dotnet-ef > /dev/null
	dotnet tool install --verbosity quiet --tool-path $(DOTNET_TOOLS) dotnet-dev-certs > /dev/null
    
create-certificate:
	sudo dotnet dev-certs https --clean
	sudo dotnet dev-certs https --export-path "$(KESTREL_CERTIFICATE_FOLDER)/aspnetapp.pfx" --password "$(KESTREL_CERTIFICATE_PASSWORD)" --trust

execute-migrations:
	dotnet ef database update --project src/dukkantek.Api