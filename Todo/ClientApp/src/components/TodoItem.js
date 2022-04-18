import React, { Component, useState } from 'react';
import { Home } from './Home';
import { Validation } from './../Validation.js'

export class TodoItem extends Component {
    static displayName = TodoItem.name;

    constructor(props) {
        super(props);
        this.state = {
            id: this.props.value.id,
            txt: this.props.value.txt,
            done: this.props.value.done,
            endDate: this.props.value.endDate,
            compDate: this.props.value.compDate,
            callback: this.props.callback,
            backColor: !this.props.value.done && (new Date(this.props.value.endDate) < new Date()) ? 'expired':''
        };
        this.CallBackFunction = this.CallBackFunction.bind(this);
    }

    CallBackFunction() {
        this.state.callback();
    }

    handleOnChange(event) {
        console.log("TodoItem handleClick", event.target.checked);
        if (event.target.checked) this.putDoneData();
        else this.putTodoData();
    }

    handleOnClick(event) {
        this.deleteData();
        console.log("TodoItem handleClick", event);
    }

    handleEditClick() {
        var value = prompt("内容を編集してください。", this.state.txt);
        //var captical = prompt("期日を編集してください。", this.state.endDate);

        
        var res = Validation.txtCheck(value, 100, 1);

        if (res.ok) {
            console.log(value);
            this.putEditData(value);
        } else {
            alert(res.errTxt);
        }
    }

    render() {
        return (
            <tr key={this.state.id} className={this.state.backColor}>
                <td><input type="checkbox" checked={this.state.done} onChange={e => this.handleOnChange(e)} /></td>
                <td>{this.state.txt}</td>
                <td>{this.state.endDate}</td>
                <td>{this.state.done ? this.state.compDate : ''}</td>
                <td><a className="button" href="#" onClick={e => this.handleEditClick(e)}>編集</a></td>
                <td><a className="button" href="#" onClick={e => this.handleOnClick(e)}>削除</a></td>
            </tr>
        );
    }

    async deleteData() {
        // サーバへ送りたいデータ
        const data = { id: this.state.id };
        // FetchAPIのオプション準備
        const param = {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            // リクエストボディ
            body: JSON.stringify(data)
        };

        // paramを付ける以外はGETと同じ
        await fetch('api/Delete', param)
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
                }
            });
    }

    async putDoneData() {
        // サーバへ送りたいデータ
        const data = { id: this.state.id};
        // FetchAPIのオプション準備
        const param = {
            method: "PUT",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            // リクエストボディ
            body: JSON.stringify(data)
        };

        // paramを付ける以外はGETと同じ
        await fetch('api/Done', param)
            .then(Response => {
                if (Response.ok) {
                    return Response.json();
                }
            })
            .then(data => {
                var json = JSON.parse(JSON.stringify(data));

                if (json.ok) {
                    // ここに何らかの処理
                    this.setState({
                        done: true,
                        compDate: json.date,
                        backColor: ''
                    }, () => {

                    });
                }
            });
    }

    async putTodoData() {
        // サーバへ送りたいデータ
        const data = { id: this.state.id };
        // FetchAPIのオプション準備
        const param = {
            method: "PUT",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            // リクエストボディ
            body: JSON.stringify(data)
        };

        // paramを付ける以外はGETと同じ
        await fetch('api/Todo', param)
            .then(Response => {
                if (Response.ok) {
                    return Response.json();
                }
            })
            .then(data => {
                var json = JSON.parse(JSON.stringify(data));

                if (json.ok) {
                    // ここに何らかの処理
                    this.setState({
                        done: false,
                        compDate: json.date,
                        backColor: (new Date(this.state.endDate) < new Date()) ? 'expired' : ''
                    }, () => {

                    });
                }
            });
    }

    async putEditData(value) {
        
        // サーバへ送りたいデータ
        const data = { id: this.state.id, txt: value };
        // FetchAPIのオプション準備
        const param = {
            method: "PUT",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },

            // リクエストボディ
            body: JSON.stringify(data)
        };

        // paramを付ける以外はGETと同じ
        await fetch('api/Edit', param)
            .then(Response => {
                if (Response.ok) {
                    return Response.json();
                }
            })
            .then(data => {
                var json = JSON.parse(JSON.stringify(data));
                console.log("json", json);
                if (json.ok) {
                    console.log("putEditData", value);
                    // ここに何らかの処理
                    this.setState({ txt: json.txt });
                }
            });
    }
}