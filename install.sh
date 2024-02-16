
#create folder
echo "ensuring folder"
mkdir -p /c/Program\ Files/Wilkywayre/Iot

# delete existing files
echo "Deleting existing files"
rm -rf /c/Program\ Files/Wilkywayre/Iot/*

# Copy files over to program files folder
echo "Copying files to program files folder"
cp -r /c/dev/wilkywayre/GoveeClient/Wilkywayre.Iot.Service/bin/Release/net8.0-windows/win-x64/publish/* /c/Program\ Files/Wilkywayre/Iot
# See if files are there
echo "Files in folder"
ls -AL /c/Program\ Files/Wilkywayre/Iot

# stop service
echo "Stopping service"
#sc stop WilkywareIotService 

# remove exisitng service
echo "Removing existing service"
#sc delete WilkywareIotService /f
 
# install service
echo "Installing service"

#sc create WilkywareIotService binPath= "C:\Program Files\Wilkywayre\Iot\Wilkywayre.Iot.Service.exe" start= auto