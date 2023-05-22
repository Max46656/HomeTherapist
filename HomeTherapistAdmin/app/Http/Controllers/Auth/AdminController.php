<?php

namespace Backpack\CRUD\app\Http\Controllers;

use Backpack\CRUD\app\Http\Controllers\AdminController as BackpackAdminController;
use Illuminate\Support\Facades\Redirect;

class AdminController extends BackpackAdminController
{
    public function dashboard()
    {
        // $user = Auth::user();

        // if ($user) {
        //     $passes = Gate::forUser($user)->check('admin');
        // }

        if (backpack_user() && !backpack_user()->hasRole('admin')) {
            return Redirect::route('backpack.auth.logout');
        }

        $this->data['title'] = trans('backpack::base.dashboard'); // set the page title
        $this->data['breadcrumbs'] = [
            trans('backpack::crud.admin') => backpack_url('dashboard'),
            trans('backpack::base.dashboard') => false,
        ];

        return view(backpack_view('dashboard'), $this->data);
    }
}
