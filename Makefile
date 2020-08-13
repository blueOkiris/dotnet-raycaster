OBJNAME :=       raycast
OBJFLDR :=       .

PROJNAME :=      dotnet-raycast
TARGET_FRMWRK := netcoreapp3.1

SRCFLDRS :=      src src/graphics src/physics
SRCFILES :=      $(foreach folder,$(SRCFLDRS),$(wildcard $(folder)/*.cs))

ifdef LINUX
    RUNTIME := linux-x64
else
    ifdef WIN32
        RUNTIME := win-x86
    else
        ifdef WIN64
            RUNTIME := win-x64
        endif
    endif
endif

$(OBJFLDR)/$(OBJNAME) : $(SRCFILES)
	dotnet publish $(PROJNAME).csproj -f $(TARGET_FRMWRK) -p:PublishSingleFile=true -r $(RUNTIME)
	mkdir -p $(OBJFLDR)
	cp bin/Debug/$(TARGET_FRMWRK)/$(RUNTIME)/publish/$(PROJNAME) $(OBJFLDR)/$(OBJNAME)
	chmod +x raycast

.PHONY : clean
clean :
	rm -rf bin
	rm -rf obj
	rm -rf $(OBJFLDR)

