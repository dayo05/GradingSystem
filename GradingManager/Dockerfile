FROM archlinux AS base

# C# Support
RUN pacman -Sy --noconfirm wget
RUN wget https://dot.net/v1/dotnet-install.sh
RUN chmod 777 dotnet-install.sh
RUN ./dotnet-install.sh -c Current

# C/C++ Support
RUN pacman -Sy --noconfirm gcc

# Rust Support
RUN curl https://sh.rustup.rs/ -sSf | sh -s -- -y

WORKDIR /app

CMD ["rm", "grad/*"]
CMD ["/root/.dotnet/dotnet", "GradingManager.dll"]

# docker build -t irooj-grading-main .
# docker run --name grading_main --network=host -v `pwd`/bin/Debug/net5.0:/app -p 8080:8080 irooj-grading-main
# docker run --name grading_main --network=host -v `pwd`/bin/Release/net5.0:/app -p 8080:8080 irooj-grading-main
