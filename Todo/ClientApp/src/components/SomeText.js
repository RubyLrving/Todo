import React, { Component, useState } from 'react';
import { Home } from './Home';
import { Validation } from './../Validation.js';

export class SomeText extends Component {
    static displayName = SomeText.name;

    constructor(props) {
        super(props);
        this.state = { task: '', date: '', callback: this.props.callback, error: false, errTxt: "" };
        this.CallBackFunction = this.CallBackFunction.bind(this);
    }

    CallBackFunction() {
        this.state.callback();
    }

    handleOnChange(event){
        this.setState({ task: event.target.value });
    }

    dateOnChange(event) {
        this.setState({ date: event.target.value });
    }

    handleKeyDown(event) {
        if (event.key === 'Enter') {
            var res = Validation.dateCheck(this.state.date);
            if (!res.ok) {
                this.setState({
                    error: !res.ok,
                    errTxt: res.errTxt,
                }, () => {
                    console.log("res", res);
                    console.log("this.state", this.state);
                });
                return;
            }

            console.log(event.target.value);
            res = Validation.txtCheck(event.target.value, 100, 1);
            
            if (res.ok) {
                this.postTaskData();
            }
            this.setState({
                error: !res.ok,
                errTxt: res.errTxt,
            }, () => {
                console.log("res", res);
                console.log("this.state", this.state);
            });

        }
    }

    render() {
        return (
            <div>
                <label>
                    期限
                    <input type="date" ref="limitDate" onChange={e => this.dateOnChange(e)} />
                </label><br />
                Add Task:<br />
                <input className="someText"
                    value={this.state.task}
                    placeholder="Add New Task"
                    onChange={e => this.handleOnChange(e)}
                    onKeyDown={e => this.handleKeyDown(e)} />
                <label className={this.state.error ? "red" : "hidden"}>{this.state.errTxt}</label>
            </div>
        );
    }

    async postTaskData() {
        // サーバへ送りたいデータ
        const data = { txt: this.state.task, date: this.state.date };
        // FetchAPIのオプション準備
        const param = {
            method: "POST",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            // リクエストボディ
            body: JSON.stringify(data)
        };
        // paramを付ける以外はGETと同じ
        await fetch('api/Add', param)
        .then(Response => {
            if (Response.ok) {
                return Response.json();
            }
        })
        .then(data => {
            var json = JSON.parse(JSON.stringify(data));
            if (json.ok) {
                // ここに何らかの処理
                this.CallBackFunction();
            } else {
                console.log("error", json.errorMsg);
            }
        });
    }
}