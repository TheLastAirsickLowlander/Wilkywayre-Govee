
#create folder
echo "ensuring folder"
mkdir -p /c/Program\ Files/Wilkywayre/Govee


# delete existing files
echo "Deleting existing files"
rm -rf /c/Program\ Files/Wilkywayre/Govee/*

# Copy files over to program files folder
echo "Copying files to program files folder"
cp -r /c/dev/wilkywayre/GoveeClient/Wilkywayre.Govee.Service/bin/Release/net8.0/win-x64/publish/* /c/Program\ Files/Wilkywayre/Govee
# See if files are there
echo "Files in folder"
ls -AL /c/Program\ Files/Wilkywayre/Govee

# stop service
echo "Stopping service"
sc stop GoveeService

# remove exisitng service
echo "Removing existing service"
sc delete GoveeService /f
 
# install service
echo "Installing service"
sc create GoveeService binPath= "C:\Program Files\Wilkywayre\Govee\Wilkywayre.Govee.Service.exe" start= auto
