use std::io;

fn main() {
    let mut s = String::new();
    io::stdin()
        .read_line(&mut s)
        .expect("Failed to read line");

    let inputs:Vec<u32> = s.split_whitespace()
        .map(|x| x.parse().expect("Input is not integar!"))
        .collect();

    println!("{}", inputs[0]+inputs[1]);
}
