FROM archlinux AS base

# Make pacman latest
RUN pacman -Syu --noconfirm

# C# Support
RUN pacman -Sy --noconfirm wget
RUN wget https://dot.net/v1/dotnet-install.sh
RUN chmod 500 dotnet-install.sh
RUN ./dotnet-install.sh -c Current

# C/C++ Support
RUN pacman -Sy --noconfirm gcc

# Rust Support
RUN curl https://sh.rustup.rs/ -sSf | sh -s -- -y

# Python Support
RUN pacman -Sy --noconfirm python3

# Aheui Support(CAheui)
RUN wget https://raw.githubusercontent.com/aheui/caheui/master/aheui.c
RUN gcc -o /root/aheui aheui.c

WORKDIR /app

RUN ls

CMD ["rm", "grad/*"]
CMD ["/root/.dotnet/dotnet", "GradingManager.dll"]

# docker build -t irooj-grading-main .
# docker run --name grading_main --network=host -v `pwd`/bin/Debug/net5.0:/app -p 8080:8080 irooj-grading-main
# docker run --name grading_main --network=host -v `pwd`/bin/Release/net5.0:/app -p 8080:8080 irooj-grading-main
