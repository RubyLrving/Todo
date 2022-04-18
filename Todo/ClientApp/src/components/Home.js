import React, { Component } from 'react';
import { SomeText } from './SomeText';
import { TodoItem } from './TodoItem';
import { Filter } from './Filter';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = { tasks: [], loading: true, filter: "All", };
        this.getTasksData = this.getTasksData.bind(this);
        this.handleFilterChange = this.handleFilterChange.bind(this);
    }

    componentDidMount() {
        this.getTasksData();
    }

    handleFilterChange(value) {
        this.setState({ filter: value }, () => {
            this.getTasksData();
        });
    }

    static renderTasksTable(tasks, callback, filter) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>完了済</th>
                        <th>内容</th>
                        <th>期日</th>
                        <th>完了日</th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {tasks.map(task => 
                        <TodoItem key={task.id} value={task} callback={callback} />
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Home.renderTasksTable(this.state.tasks, this.getTasksData, this.state.filter);

        return (
            <div>
                <h1>ToDo</h1>
                <SomeText callback={this.getTasksData} />
                <Filter value={this.state.filter} callback={this.handleFilterChange} />
                {contents}
            </div>
        );
    }

    async getTasksData() {
        const query_params = new URLSearchParams({ Filter: this.state.filter });

        await fetch('api/Select?' + query_params)
            .then(Response => {
                if (Response.ok) {
                    return Response.json();
                }
            })
            .then(data => {
                var json = JSON.parse(JSON.stringify(data));
                if (json.ok) {
                    json.list.sort((a, b) => {
                        var date1 = new Date(a.endDate);
                        var date2 = new Date(b.endDate);
                        var r = 0;
                        if (date1 < date2) { r = -1; }
                        else if (date1 > date2) { r = 1; }
                        return r;
                    });
                    this.setState({ tasks: json.list, loading: false, });
                } else {
                    console.log("error", json.errorMsg);
                }
            });
    }
}