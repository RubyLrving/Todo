import React, { Component } from 'react';

export class Validation extends Component {
    static txtCheck(txt, max_len, min_len) {
        var ok = true;
        var error = "";
        if (typeof max_len !== "undefined") {
            if (txt.length > max_len) {
                ok = false;
                error = "文字数を" + max_len + "以内にしてください。";
            }
        }
        if (typeof min_len !== "undefined") {
            if (txt.length < min_len) {
                ok = false;
                error = "文字数を" + min_len + "以上にしてください。";
            }
        }

        return {
            ok: ok,
            errTxt: error,
        };
    }

    static dateCheck(txt) {
        var ok = true;
        var error = "";

        if (txt == "") {
            ok = false;
            error = "日付を入力してください。";
        }

        return {
            ok: ok,
            errTxt: error,
        };
    }
}