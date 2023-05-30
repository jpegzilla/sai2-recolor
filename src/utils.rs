

pub struct SearchAndReplace {
    search: String,
    replace: String,
}

pub fn correct_hex_color(hex: &str, seq_len: usize) -> String {
    let mut subs = Vec::with_capacity(hex.len() / seq_len);
    let mut iter = hex.chars();
    let mut pos = 0;

    while pos < hex.len() {
        let mut len = 0;

        for ch in iter.by_ref().take(seq_len) {
            len += ch.len_utf8();
        }

        subs.push(&hex[pos..pos + len]);
        pos += len;
    }

    subs.into_iter()
        .rev()
        .fold(String::new(), |a, b| a + b)
}

// ruby:
// def correct_hex_color(hex, len)
//     hex.chars.each_slice(len).map(&:join).reverse().join()
// end