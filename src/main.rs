#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")] // hide console window on Windows in release

mod utils;

use eframe::egui;

fn main() -> Result<(), eframe::Error> {
    let options = eframe::NativeOptions {
        initial_window_size: Some(egui::vec2(480.0, 640.0)),
        ..Default::default()
    };

    print!("{:?}", utils::sai2_recolor_utils::SearchAndReplace);

    eframe::run_native(
        "sai2_recolor - theme and color overhaul for paint tool sai 2",
        options,
        Box::new(|_cc| Box::<Sai2Recolor>::default()),
    )
}

struct Sai2Recolor {
    name: String,
    age: u32,
}

impl Default for Sai2Recolor {
    fn default() -> Self {
        Self {
            name: "Cock".to_owned(),
            age: 42,
        }
    }
}

impl eframe::App for Sai2Recolor {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        egui::CentralPanel::default().show(ctx, |ui| {
            ui.heading("sai2_recolor");
            ui.horizontal(|ui| {
                let name_label = ui.label("Your name: ");
                ui.text_edit_singleline(&mut self.name)
                    .labelled_by(name_label.id);
            });
            ui.add(egui::Slider::new(&mut self.age, 0..=120).text("age"));
            if ui.button("Click each year").clicked() {
                self.age += 1;
            }
            ui.label(format!("Hello '{}', age {}", self.name, self.age));
        });
    }
}