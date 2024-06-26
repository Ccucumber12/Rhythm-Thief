from mido import MidiFile
from PIL import Image, ImageDraw
import json


mid = MidiFile('./stage1.mid')
assert len([msg for msg in mid if msg.type == 'set_tempo']) == 1
midi_tempo = [msg for msg in mid if msg.type == 'set_tempo'][0].dict()['tempo']
ticks_per_beat = mid.ticks_per_beat
beats_per_measure = 4
# ref: https://mido.readthedocs.io/en/stable/files/midi.html?highlight=ticks_per_beat#midi-tempo-vs-bpm
full_length = mid.length


class SheetRendering:
    def __init__(self, grid_width_per_sec, grid_height, num_of_track, full_length):
        self.grid_width_per_sec = int(grid_width_per_sec)
        self.grid_height = int(grid_height)
        self.num_of_track = num_of_track
        self.full_width = int(grid_width_per_sec * full_length)
        self.full_height = int(grid_height * num_of_track)
        self.image = Image.new('RGBA', (self.full_width, self.full_height), (255, 255, 255, 0))
        # self.json_output = {}
        self.json_output2 = {}

    def draw_measure_bar(self):
        sec_per_measure = tick2sec(ticks_per_beat) * beats_per_measure
        tmp_img = Image.new('RGBA', (self.full_width, self.full_height), (255, 255, 255, 0))
        for i in range(int(full_length / sec_per_measure) + 1):
            draw = ImageDraw.Draw(tmp_img)
            x = i * sec_per_measure * self.grid_width_per_sec
            draw.line([(x, 0), (x, self.full_height)], fill='#8888', width=5)
        self.image = Image.alpha_composite(self.image, tmp_img)

    def __draw_track_line(self, draw_index, target_image=None):
        if target_image is None:
            target_image = self.image

        draw = ImageDraw.Draw(target_image)
        # start_x = self.grid_height // 2
        # end_x = self.full_width - self.grid_height // 2
        start_x = 0
        end_x = self.full_width
        y = int((draw_index + 0.5) * self.grid_height)
        draw.line([(start_x, y), (end_x, y)], fill='black', width=5)

    def draw_track_single(self, draw_index, track, icon):
        # self.__draw_track_line(draw_index)
        curr_tick = 0
        # self.json_output[track.name] = {
        #     'name': track.name,
        #     'type': 'single',
        #     'data': [],
        # }
        self.json_output2[track.name] = []
        for msg in track:
            curr_tick += msg.time
            if msg.type == 'note_on':
                curr_time = tick2sec(curr_tick)
                x = int(curr_time * self.grid_width_per_sec)
                y = int(draw_index * self.grid_height)
                self.image.alpha_composite(icon, (x, y))
                # self.json_output[track.name]['data'].append(curr_time)
                self.json_output2[track.name].append(curr_time)

    def draw_track_duration_two_track(self, draw_index, track_on, track_off, icon):
        # self.__draw_track_line(draw_index)

        all_msgs = []

        curr_tick = 0
        self.json_output2[track_on.name] = []
        for msg in track_on:
            curr_tick += msg.time
            if msg.type == 'note_on':
                curr_time = tick2sec(curr_tick)
                all_msgs.append((curr_time, 'on'))
                self.json_output2[track_on.name].append(curr_time)

        curr_tick = 0
        self.json_output2[track_off.name] = []
        for msg in track_off:
            curr_tick += msg.time
            if msg.type == 'note_on':
                curr_time = tick2sec(curr_tick)
                all_msgs.append((curr_time, 'off'))
                self.json_output2[track_off.name].append(curr_time)

        all_msgs.sort()

        start_time = None
        # self.json_output[track_on.name] = {
        #     'name': track_on.name,
        #     'type': 'duration',
        #     'data': [],
        # }
        for msg in all_msgs:
            if msg[1] == 'on':
                start_time = msg[0]
            elif msg[1] == 'off':
                end_time = msg[0]

                draw = ImageDraw.Draw(self.image)
                start_x = int(start_time * self.grid_width_per_sec)
                end_x = int(end_time * self.grid_width_per_sec)
                y = int((draw_index + 0.5) * self.grid_height)
                draw.line([(start_x, y), (end_x, y)], fill='lightgray', width=20)

                x = int(start_time * self.grid_width_per_sec)
                y = int(draw_index * self.grid_height)
                self.image.alpha_composite(icon, (x, y))
                # self.json_output[track_on.name]['data'].append((start_time, end_time))

                start_time, end_time = None, None
            else:
                raise ValueError()

    def output_sheet(self, filename_png, filename_json):
        self.image.show()
        self.image.save(filename_png)

        # json.dump(self.json_output, open(filename_json, 'w'))
        json.dump(self.json_output2, open(filename_json, 'w'))

    def output_background(self, filename_png):
        img = Image.new('RGBA', (self.full_width, self.full_height), (255, 255, 255, 255))
        for i in range(self.num_of_track):
            self.__draw_track_line(i, img)

        img.save(filename_png)

def tick2sec(tick):
    return tick * midi_tempo / 1000000 / ticks_per_beat


def process_track(track):
    # for msg in track:
    #     print('   ', msg)
    curr_tick = 0
    ret = []
    for msg in track:
        print(msg)
        curr_tick += msg.time
        if msg.type == 'note_on':
            ret.append(('on', tick2sec(curr_tick)))
        elif msg.type == 'note_off':
            ret.append(('off', tick2sec(curr_tick)))
    # print(ret)


def test():
    # for i, track in enumerate(mid.tracks):
    #     print('Track {}: {}'.format(i, track.name))

    #     # check if contains any note_on or note_off message
    #     if sum([msg.type.startswith('note_') for msg in track]) > 0:
    #         process_track(track)

    #     print()

    image = Image.new('RGBA', (400, 400), (255, 255, 255, 255))
    image.alpha_composite(Image.open('images/emoji_u1f3c3.png'), (100, 100))
    image.show()


def main():
    tracks = {track.name: track for track in mid.tracks}

    width_per_second = 128
    emoji_size = 64

    sheet = SheetRendering(width_per_second, emoji_size, 4, full_length)
    sheet.draw_track_single(0, tracks['move'], Image.open('images/emoji_u1f3c3.png').resize((emoji_size, emoji_size)))
    sheet.draw_track_duration_two_track(1, tracks['lightsOff'], tracks['lightsOn'], Image.open('images/emoji_u1f4a1.png').resize((emoji_size, emoji_size)))
    sheet.draw_track_duration_two_track(2, tracks['gateOpen'], tracks['gateClose'], Image.open('images/emoji_u1f6aa.png').resize((emoji_size, emoji_size)))
    sheet.draw_track_duration_two_track(3, tracks['bellRing'], tracks['bellStop'], Image.open('images/emoji_u1f514.png').resize((emoji_size, emoji_size)))
    #sheet.draw_track_single(4, tracks['gun'], Image.open('images/emoji_u1f4a5.png').resize((emoji_size, emoji_size)))
    sheet.draw_measure_bar()
    sheet.output_background('background.png')
    sheet.output_sheet('sheet.png', 'output.json')


if __name__ == '__main__':
    # test()
    main()
